#ifndef __DT_CPP_HELPER_H__
#define __DT_CPP_HELPER_H__

#include <string>

#include "dt_core.h"
#include "dt_query.h"

// These C++ classes are some very simple wrapper of core APIs,
// some operations may be quite convenient in the C++ way

namespace dt {

class DT_API DatatreeParsingException {

public:
	DatatreeParsingException(const char* _msg);

	const char* msg;

};

class DT_API Value {

public:
	Value();
	~Value();

	Value(dt_datatree_t dt, dt_value_t val);
	Value(dt_datatree_t dt, const char* fmt, ...);
	Value(const Value &other);
	Value &operator = (const Value &other);

	bool operator == (const Value &other) const;
	bool operator != (const Value &other) const;
	bool operator < (const Value &other) const;

	bool equals(const dt_value_t val) const;
	bool equals(const Value &other) const;
	int compare(const dt_value_t val, dt_bool_t numRawCmp = DT_TRUE) const;
	int compare(const Value &other, dt_bool_t numRawCmp = DT_TRUE) const;

	bool isNull(void) const;
	void setNull(void);

	void cloneFrom(dt_datatree_t dt, dt_value_t val);
	void cloneFrom(const Value &other);

	void unsafeSwap(dt_value_t val);
	void unsafeMove(dt_value_t val);

	dt_datatree_t getDatatree(void);
	dt_value_t getValue(void);

	operator dt_value_t(void);

	std::string toString(dt_bool_t compact = DT_FALSE) const;
	void toString(std::string &buf, dt_bool_t compact = DT_FALSE) const;

	template<typename T>
	T as(void) const {
		DT_ASSERT(0 && "Impossible");
	}

private:
	void swap(Value &other);
	void release(void);
	void destroy(void);

private:
	dt_datatree_t mDatatree;
	dt_value_t mRep;
	unsigned int* mUseCount;

};

class DT_API ValueRef {

public:
    ValueRef();
    ValueRef(dt_datatree_t d, dt_value_t v);
    ~ValueRef();

    dt_value_t getValue(void);

    void setValue(dt_value_t v);
    void setValue(const char* v);
    void setValue(const std::string &v);

    dt_type_t getType(void) const;

    bool isNull(void) const;
    bool isInteger(void) const;
    bool isFloat(void) const;
    bool isNumber(void) const;

    template<typename T>
    bool is(void) const {
        DT_ASSERT(0 && "Impossible");
    }

    template<typename T>
    T as(void) const {
        return (T)as<long>();
    }

    template<typename T>
    T to(void) const {
        DT_ASSERT(0 && "Impossible");
    }

private:
    dt_datatree_t dt;
    dt_value_t val;

};

class DT_API Obj {

public:
    Obj();
    Obj(dt_datatree_t d, const Value &a);
    Obj(dt_datatree_t d, const ValueRef &a);
    ~Obj();

    bool invalid(void) const;

    dt_object_t getObject(void);

    size_t count(void) const;

    ValueRef operator [] (size_t index) const;
    ValueRef operator [] (dt_value_t k) const;
    ValueRef operator [] (const char* k) const;

    void at(size_t index, ValueRef &_k, ValueRef &_v) const;

    bool exists(dt_value_t k) const;
    bool exists(const char* k) const;
    
private:
    dt_datatree_t dt;
    dt_object_t raw;
    Value val;
    
};

class DT_API Arr {
    
public:
    Arr();
    Arr(dt_datatree_t d, const Value &a);
    Arr(dt_datatree_t d, const ValueRef &a);
    ~Arr();
    
    dt_array_t getArray(void);
    
    size_t count(void) const;
    
    ValueRef operator [] (size_t index) const;
    
    ValueRef first(void) const;
    ValueRef last(void) const;
    
private:
    dt_datatree_t dt;
    dt_array_t raw;
    Value val;
    
};

template<> inline dt_bool_t Value::as(void) const { dt_bool_t ret = DT_FALSE; dt_value_data_as(&ret, mRep, DT_BOOL); return ret; }
template<> inline bool Value::as(void) const { dt_bool_t ret = DT_FALSE; dt_value_data_as(&ret, mRep, DT_BOOL); return !!ret; }
template<> inline int Value::as(void) const { long ret = 0; dt_value_data_as(&ret, mRep, DT_LONG); return (int)ret; }
template<> inline long Value::as(void) const { long ret = 0; dt_value_data_as(&ret, mRep, DT_LONG); return ret; }
template<> inline float Value::as(void) const { double ret = 0; dt_value_data_as(&ret, mRep, DT_DOUBLE); return (float)ret; }
template<> inline double Value::as(void) const { double ret = 0; dt_value_data_as(&ret, mRep, DT_DOUBLE); return ret; }
template<> inline std::string Value::as(void) const { char* ret = NULL; dt_value_data_as(&ret, mRep, DT_STRING); return ret; }
template<> inline dt_object_t Value::as(void) const { dt_object_t ret = NULL; dt_value_data_as(&ret, mRep, DT_OBJECT); return ret; }
template<> inline dt_array_t Value::as(void) const { dt_array_t ret = NULL; dt_value_data_as(&ret, mRep, DT_ARRAY); return ret; }

template<> inline Obj Value::as(void) const { return Obj(DT_DUMMY_DATATREE, *this); }
template<> inline Arr Value::as(void) const { return Arr(DT_DUMMY_DATATREE, *this); }

template<> inline bool ValueRef::is<dt_bool_t>(void) const { return dt_value_type(val) == DT_BOOL; }
template<> inline bool ValueRef::is<bool>(void) const { return dt_value_type(val) == DT_BOOL; }
template<> inline bool ValueRef::is<int>(void) const { return dt_value_type(val) == DT_LONG; }
template<> inline bool ValueRef::is<long>(void) const { return dt_value_type(val) == DT_LONG; }
template<> inline bool ValueRef::is<float>(void) const { return dt_value_type(val) == DT_DOUBLE; }
template<> inline bool ValueRef::is<double>(void) const { return dt_value_type(val) == DT_DOUBLE; }
template<> inline bool ValueRef::is<char*>(void) const { return dt_value_type(val) == DT_STRING; }
template<> inline bool ValueRef::is<const char*>(void) const { return dt_value_type(val) == DT_STRING; }
template<> inline bool ValueRef::is<std::string>(void) const { return dt_value_type(val) == DT_STRING; }
template<> inline bool ValueRef::is<dt_object_t>(void) const { return dt_value_type(val) == DT_OBJECT; }
template<> inline bool ValueRef::is<dt_array_t>(void) const { return dt_value_type(val) == DT_ARRAY; }
template<> inline bool ValueRef::is<Obj>(void) const { return dt_value_type(val) == DT_OBJECT; }
template<> inline bool ValueRef::is<Arr>(void) const { return dt_value_type(val) == DT_ARRAY; }

template<> inline dt_bool_t ValueRef::as(void) const { dt_bool_t ret = DT_FALSE; dt_value_data_as(&ret, val, DT_BOOL); return ret; }
template<> inline bool ValueRef::as(void) const { dt_bool_t ret = DT_FALSE; dt_value_data_as(&ret, val, DT_BOOL); return !!ret; }
template<> inline int ValueRef::as(void) const { long ret = 0; dt_value_data_as(&ret, val, DT_LONG); return (int)ret; }
template<> inline long ValueRef::as(void) const { long ret = 0; dt_value_data_as(&ret, val, DT_LONG); return ret; }
template<> inline float ValueRef::as(void) const { double ret = 0; dt_value_data_as(&ret, val, DT_DOUBLE); return (float)ret; }
template<> inline double ValueRef::as(void) const { double ret = 0; dt_value_data_as(&ret, val, DT_DOUBLE); return ret; }
template<> inline char* ValueRef::as(void) const { char* ret = NULL; dt_value_data_as(&ret, val, DT_STRING); return ret; }
template<> inline const char* ValueRef::as(void) const { char* ret = NULL; dt_value_data_as(&ret, val, DT_STRING); return ret; }
template<> inline std::string ValueRef::as(void) const { char* ret = NULL; dt_value_data_as(&ret, val, DT_STRING); return ret; }
template<> inline dt_object_t ValueRef::as(void) const { dt_object_t ret = NULL; dt_value_data_as(&ret, val, DT_OBJECT); return ret; }
template<> inline dt_array_t ValueRef::as(void) const { dt_array_t ret = NULL; dt_value_data_as(&ret, val, DT_ARRAY); return ret; }
template<> inline Obj ValueRef::as(void) const { return Obj(dt, *this); }
template<> inline Arr ValueRef::as(void) const { return Arr(dt, *this); }

template<> inline bool ValueRef::to(void) const {
    if(is<bool>())
        return as<bool>();
    if(is<long>())
        return !!as<long>();
    if(is<double>())
        return !!as<double>();

    return true;
}

class DT_API Command {

public:
	Command();
	Command(const char* fmt, ...);
	~Command();

	int parse(const char* fmt, ...);

	void clear(void);

	Value query(dt_value_t t);
	dt_query_status_t query(dt_value_t t, Value &ret);
	dt_query_status_t query(dt_value_t t, dt_value_t &ret);

private:
	Command(const Command &);
	Command &operator = (const Command &);

private:
	dt_command_t mCommand;

};

}

#endif // __DT_CPP_HELPER_H__
