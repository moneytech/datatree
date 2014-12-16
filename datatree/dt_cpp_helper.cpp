#include <stdexcept>
#include "dt_cpp_helper.hpp"

namespace dt {

static void _on_parse_error(dt_enum_compatible_t status, const char* msg, const char* pos, size_t row, size_t col) {
	printf(
		"Parsing error.\nError code: %d, error message: %s\nRow: %lu, col: %lu\nText: %s...\n",
		status,
		msg,
		row,
		col,
		pos
	);
}

DatatreeParsingException::DatatreeParsingException(const char* _msg) : msg(_msg) {
}

Value::Value() : mDatatree(NULL), mRep(NULL), mUseCount(NULL) {
}

Value::~Value() {
	release();
}

Value::Value(dt_datatree_t dt, dt_value_t val) : mDatatree(dt), mRep(val), mUseCount(new unsigned int(1)) {
}

Value::Value(const Value &other) {
	mDatatree = other.mDatatree;
	mRep = other.mRep;
	mUseCount = other.mUseCount;
	if(mUseCount)
		++(*mUseCount);
}

Value::Value(dt_datatree_t dt, const char* fmt, ...) : mUseCount(new unsigned int(1)) {
	mDatatree = dt;

	va_list argptr;
	va_start(argptr, fmt);
	if(fmt) {
		char buf[DT_STR_LEN];
		vsprintf(buf, fmt, argptr);
		dt_create_value(dt, &mRep, _on_parse_error, buf);
	} else {
		dt_type_t t = (dt_type_t)va_arg(argptr, dt_enum_compatible_t);
		switch(t) {
		case DT_NULL:
			dt_create_value(dt, &mRep, _on_parse_error, NULL, DT_NULL);
			break;
		case DT_BOOL:
			dt_create_value(dt, &mRep, _on_parse_error, NULL, t, (dt_bool_t)va_arg(argptr, dt_enum_compatible_t));
			break;
		case DT_LONG:
			dt_create_value(dt, &mRep, _on_parse_error, NULL, t, (long)va_arg(argptr, long));
			break;
		case DT_DOUBLE:
			dt_create_value(dt, &mRep, _on_parse_error, NULL, t, va_arg(argptr, double));
			break;
		case DT_STRING: {
				const char* str = va_arg(argptr, const char*);
				dt_create_value(dt, &mRep, _on_parse_error, NULL, t, str);
			}
			break;
		case DT_OBJECT:
			DT_ASSERT(0 && "Not supported");
			break;
		case DT_ARRAY:
			DT_ASSERT(0 && "Not supported");
			break;
		default:
			DT_ASSERT(0 && "Invalid type");
		}
	}
	va_end(argptr);
}

Value &Value::operator = (const Value &other) {
	if(mRep == other.mRep)
        return *this;
    Value tmp(other);
    swap(tmp);

    return *this;
}

bool Value::operator == (const Value &other) const {
	return mRep == other.mRep;
}

bool Value::operator != (const Value &other) const {
	return mRep != other.mRep;
}

bool Value::operator < (const Value &other) const {
	return mRep < other.mRep;
}

bool Value::equals(const dt_value_t val) const {
	return dt_value_compare(mRep, val, DT_TRUE) == 0;
}

bool Value::equals(const Value &other) const {
	return dt_value_compare(mRep, other.mRep, DT_TRUE) == 0;
}

int Value::compare(const dt_value_t val, dt_bool_t numRawCmp/* = DT_TRUE*/) const {
	return dt_value_compare(mRep, val, DT_TRUE);
}

int Value::compare(const Value &other, dt_bool_t numRawCmp/* = DT_TRUE*/) const {
	return dt_value_compare(mRep, other.mRep, DT_TRUE);
}

bool Value::isNull(void) const {
	return mRep == 0;
}

void Value::setNull(void) {
    if(mRep) {
        release();
        mRep = 0;
        mUseCount = 0;
    }
}

void Value::cloneFrom(dt_datatree_t dt, dt_value_t val) {
	release();
	mDatatree = dt;
	dt_create_value(mDatatree, &mRep, _on_parse_error, NULL, DT_NULL);
	dt_clone_value(mDatatree, val, mRep);
	mUseCount = new unsigned int(1);
}

void Value::cloneFrom(const Value &other) {
	release();
	cloneFrom(other.mDatatree, other.mRep);
}

void Value::unsafeSwap(dt_value_t val) {
	dt_value_mem_swap(mRep, val);
}

void Value::unsafeMove(dt_value_t val) {
	dt_value_mem_move(mRep, val);
}

dt_datatree_t Value::getDatatree(void) const {
	return mDatatree;
}

dt_value_t Value::getValue(void) const {
	return mRep;
}

Value::operator dt_value_t(void) {
	return mRep;
}

std::string Value::toString(dt_bool_t compact/* = DT_FALSE*/, dt_bool_t json_mode/* = DT_FALSE*/) const {
	std::string ret;
	toString(ret, compact, json_mode);

	return ret;
}

void Value::toString(std::string &buf, dt_bool_t compact/* = DT_FALSE*/, dt_bool_t json_mode/* = DT_FALSE*/) const {
	if(!mRep) return;
	char* p = NULL;
	dt_format_value(mDatatree, mRep, &p, (compact ? DT_TRUE : DT_FALSE), json_mode);
	buf = p;
	dt_free((void**)&p);
}

void Value::swap(Value &other) {
	std::swap(mDatatree, other.mDatatree);
	std::swap(mRep, other.mRep);
	std::swap(mUseCount, other.mUseCount);
}

void Value::release(void) {
	bool destroyThis = false; {
        if(mUseCount) {
            if(--(*mUseCount) == 0) {
                destroyThis = true;
            }
        }
    }
    if(destroyThis)
        destroy();
}

void Value::destroy(void) {
	dt_destroy_value(mDatatree, mRep);
	delete mUseCount;
}

ValueRef::ValueRef() {
}

ValueRef::ValueRef(dt_datatree_t d, dt_value_t v) : dt(d), val(v) {
}

ValueRef::~ValueRef() {
}

dt_value_t ValueRef::getValue(void) const {
    return val;
}

void ValueRef::setValue(dt_value_t v) {
    dt_value_mem_swap(val, v);
}

void ValueRef::setValue(const char* v) {
    dt_value_t dv = NULL;
    dt_create_value(dt, &dv, NULL, v);
    setValue(dv);
    dt_destroy_value(dt, dv);
}

void ValueRef::setValue(const std::string &v) {
    setValue(v.c_str());
}

dt_type_t ValueRef::getType(void) const {
    return dt_value_type(val);
}

bool ValueRef::isNull(void) const {
    return dt_value_type(val) == DT_NULL;
}

bool ValueRef::isInteger(void) const {
    dt_type_t t = dt_value_type(val);

    return t == DT_LONG;
}

bool ValueRef::isFloat(void) const {
    dt_type_t t = dt_value_type(val);

    return t == DT_DOUBLE;
}

bool ValueRef::isNumber(void) const {
    dt_type_t t = dt_value_type(val);

    return t >= DT_LONG || t <= DT_DOUBLE;
}

Value ValueRef::clone(void) const {
    Value result;
    result.cloneFrom(DT_DUMMY_DATATREE, getValue());

    return result;
}

Obj::Obj() : dt(NULL), raw(NULL) {
}

Obj::Obj(dt_datatree_t d, const Value &a) : dt(d) {
    dt_value_data_as(&raw, (const_cast<Value &>(a)).getValue(), DT_OBJECT);
    val = a;
}

Obj::Obj(dt_datatree_t d, const ValueRef &a) : dt(d) {
    dt_value_data_as(&raw, (const_cast<ValueRef &>(a)).getValue(), DT_OBJECT);
}

Obj::~Obj() {
}

bool Obj::invalid(void) const {
    return raw == NULL;
}

dt_object_t Obj::getObject(void) {
    return raw;
}

size_t Obj::count(void) const {
    size_t ret = 0;
    dt_object_member_count(dt, raw, &ret);

    return ret;
}

ValueRef Obj::operator [] (size_t index) const {
    dt_value_t v = NULL;
    dt_object_member_at(dt, raw, index, NULL, &v);
    if(!v)
        throw std::runtime_error("Invalid datatree value");

    return ValueRef(dt, v);
}

ValueRef Obj::operator [] (dt_value_t k) const {
    dt_value_t v = NULL;
    dt_find_object_member_by_key(dt, raw, k, &v, NULL);
    if(!v)
        throw std::runtime_error("Invalid datatree value");

    return ValueRef(dt, v);
}

ValueRef Obj::operator [] (const char* k) const {
    Value key(dt, k);
    dt_value_t v = NULL;
    dt_find_object_member_by_key(dt, raw, key.getValue(), &v, NULL);
    if(!v)
        throw std::runtime_error("Invalid datatree value");

    return ValueRef(dt, v);
}

void Obj::at(size_t index, ValueRef &_k, ValueRef &_v) const {
    dt_value_t k = NULL;
    dt_value_t v = NULL;
    dt_object_member_at(dt, raw, index, &k, &v);
    if(!k || !v)
        throw std::runtime_error("Invalid datatree value");

    _k = ValueRef(dt, k);
    _v = ValueRef(dt, v);
}

bool Obj::exists(dt_value_t k) const {
    size_t i = 0;
    dt_find_object_member_by_key(dt, raw, k, NULL, &i);

    return i != DT_INVALID_INDEX;
}

bool Obj::exists(const char* k) const {
    Value key(dt, k);
    size_t i = 0;
    dt_find_object_member_by_key(dt, raw, key.getValue(), NULL, &i);
    
    return i != DT_INVALID_INDEX;
}

Arr::Arr() : dt(NULL), raw(NULL) {
}

Arr::Arr(dt_datatree_t d, const Value &a) : dt(d) {
    dt_value_data_as(&raw, (const_cast<Value &>(a)).getValue(), DT_ARRAY);
    val = a;
}

Arr::Arr(dt_datatree_t d, const ValueRef &a) : dt(d) {
    dt_value_data_as(&raw, (const_cast<ValueRef &>(a)).getValue(), DT_ARRAY);
}

Arr::~Arr() {
}

dt_array_t Arr::getArray(void) {
    return raw;
}

size_t Arr::count(void) const {
    if(!dt || !raw)
        return 0;

    size_t ret = 0;
    dt_array_elem_count(dt, raw, &ret);

    return ret;
}

ValueRef Arr::operator [] (size_t index) const {
    dt_value_t v = NULL;
    dt_array_elem_at(dt, raw, index, &v);
    if(!v)
        throw std::runtime_error("Invalid datatree value");

    return ValueRef(dt, v);
}

ValueRef Arr::first(void) const {
    return operator [] (0);
}

ValueRef Arr::last(void) const {
    return operator [] (count() - 1);
}

Command::Command() {
	dt_create_command(&mCommand);
}

Command::Command(const char* fmt, ...) {
	char buf[DT_CMD_STR_LEN];
	va_list argptr;
	DT_ASSERT(fmt);
	va_start(argptr, fmt);
	vsprintf(buf, fmt, argptr);
	va_end(argptr);
	dt_create_command(&mCommand);
	dt_parse_command(mCommand, _on_parse_error, buf);
}

Command::~Command() {
	dt_destroy_command(mCommand);
}

int Command::parse(const char* fmt, ...) {
	char buf[DT_CMD_STR_LEN];
	va_list argptr;
	DT_ASSERT(fmt);
	va_start(argptr, fmt);
	vsprintf(buf, fmt, argptr);
	va_end(argptr);

	return dt_parse_command(mCommand, _on_parse_error, buf);
}

void Command::clear(void) {
	dt_clear_command(mCommand);
}

Value Command::query(dt_value_t t) {
	Value value;
	query(t, value);

	return value;
}

dt_query_status_t Command::query(dt_value_t t, Value &ret) {
	dt_value_t value = NULL;
	dt_query_status_t result = query(t, value);
	if(result == DTQ_GOT_REF)
		ret.cloneFrom(DT_DUMMY_DATATREE, value);
	else if(result == DTQ_GOT_NOREF)
		ret = Value(DT_DUMMY_DATATREE, value);

	return result;
}

dt_query_status_t Command::query(dt_value_t t, dt_value_t &ret) {
	dt_query_status_t result = dt_query(t, mCommand, &ret);

	return result;
}

}
