#include "dt_eval.hpp"

#ifndef DTEVER
#	define DTEVER_MAJOR	0
#	define DTEVER_MINOR	9
#	define DTEVER_PATCH	0
#	define DTEVER ((DTEVER_MAJOR * 0x01000000) + (DTEVER_MINOR * 0x00010000) + (DTEVER_PATCH))
#endif /* DTEVER */

namespace dt {

static bool isLong(double d) {
    long l = (long)d;

    return d == (double)l;
}

static Value numberResult(double d) {
    if(isLong(d))
        return Value(DT_DUMMY_DATATREE, NULL, DT_LONG, (long)d);

    return Value(DT_DUMMY_DATATREE, NULL, DT_DOUBLE, d);
}

static Value evalFunc(Eval* e, const Arr &a) {
    Value result;

    std::string name = a[1].as<std::string>();
    Arr args = a[2].as<Arr>();
    Arr body = a[3].as<Arr>();

    e->reg(name, FuncDef(args, body));

    return result;
}

static Value evalPrint(Eval* e, const Arr &a) {
    ValueRef t = a[1];
    Value v = e->eval(t);
    if(v.is<std::string>())
        printf("%s\n", v.as<std::string>().c_str());
    else if(v.is<double>())
        printf("%f\n", v.as<double>());
    else if(v.is<long>())
        printf("%ld\n", v.as<long>());
    else if(v.is<bool>())
        printf("%s\n", v.as<bool>() ? "true" : "false");

    return Value();
}

static Value evalIf(Eval* e, const Arr &a) {
    ValueRef cond = a[1];
    if(e->evalBool(cond)) {
        ValueRef tt = a[2];

        return e->eval(tt);
    } else {
        if(a.count() >= 4) {
            ValueRef tf = a[3];

            return e->eval(tf);
        }
    }

    return Value();
}

static Value evalWhile(Eval* e, const Arr &a) {
    Value result;
    ValueRef cond = a[1];
    if(e->evalBool(cond)) {
        ValueRef t = a[2];
        result = e->eval(t);
    }

    return result;
}

static Value evalVar(Eval* e, const Arr &a) {
    ValueRef vn = a[1];
    std::string vnn = vn.as<std::string>();
    Value v;
    if(a.count() >= 3) {
        ValueRef vr = a[2];
        v = e->eval(vr);
    }

    e->setVar(vnn, v);

    return v;
}

static Value evalSet(Eval* e, const Arr &a) {
    ValueRef t = a[1];
    ValueRef v = a[2];
    Value ev = e->eval(v);
    std::string vn = t.as<std::string>();
    e->setVar(vn, ev);

    return ev;
}

static Value evalGet(Eval* e, const Arr &a) {
    Value result;
    ValueRef t = a[1];
    std::string vn = t.as<std::string>();
    ValueRef v = e->getVar(vn);
    result.cloneFrom(DT_DUMMY_DATATREE, v.getValue());

    return result;
}

static Value evalAdd(Eval* e, const Arr &a) {
    ValueRef p0 = a[1];
    ValueRef p1 = a[2];
    Value l = e->eval(p0);
    Value r = e->eval(p1);
    if((l.is<double>() || l.is<long>()) && (r.is<double>() || r.is<long>())) {
        double ret = l.as<double>() + r.as<double>();

        return numberResult(ret);
    } else if(l.is<std::string>() && (r.is<std::string>() || r.is<long>() || r.is<long>())) {
        std::string ret = l.as<std::string>() + r.toString();

        return Value(DT_DUMMY_DATATREE, ret.c_str());
    } else if(r.is<std::string>() && (l.is<std::string>() || l.is<long>() || l.is<long>())) {
        std::string ret = l.toString() + r.as<std::string>();

        return Value(DT_DUMMY_DATATREE, ret.c_str());
    }

    return Value();
}

static Value evalMin(Eval* e, const Arr &a) {
    ValueRef p0 = a[1];
    ValueRef p1 = a[2];
    Value l = e->eval(p0);
    Value r = e->eval(p1);
    double ret = l.as<double>() - r.as<double>();

    return numberResult(ret);
}

static Value evalMul(Eval* e, const Arr &a) {
    ValueRef p0 = a[1];
    ValueRef p1 = a[2];
    Value l = e->eval(p0);
    Value r = e->eval(p1);
    double ret = l.as<double>() * r.as<double>();

    return numberResult(ret);
}

static Value evalDiv(Eval* e, const Arr &a) {
    ValueRef p0 = a[1];
    ValueRef p1 = a[2];
    Value l = e->eval(p0);
    Value r = e->eval(p1);
    double ret = l.as<double>() / r.as<double>();

    return numberResult(ret);
}

static Value evalMod(Eval* e, const Arr &a) {
    ValueRef p0 = a[1];
    ValueRef p1 = a[2];
    Value l = e->eval(p0);
    Value r = e->eval(p1);
    long ret = l.as<long>() % r.as<long>();

    return numberResult(ret);
}

static Value evalEq(Eval* e, const Arr &a) {
    ValueRef p0 = a[1];
    ValueRef p1 = a[2];
    Value l;
    Value r;
    bool deep = false;
    if(a.count() == 4)
        deep = a[3].as<bool>();
    if(deep) {
        l = e->eval(p0);
        r = e->eval(p1);
    } else {
        l = p0.clone();
        r = p1.clone();
    }

    if((l.is<double>() || l.is<long>()) && (r.is<double>() || r.is<long>())) {
        bool ret = l.as<double>() == r.as<double>();

        return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
    } else if(l.is<std::string>() && r.is<std::string>()) {
        bool ret = l.as<std::string>() == r.as<std::string>();

        return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
    }

    bool ret = l.equals(r);
    
    return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
}

static Value evalNotEq(Eval* e, const Arr &a) {
    ValueRef p0 = a[1];
    ValueRef p1 = a[2];
    Value l;
    Value r;
    bool deep = false;
    if(a.count() == 3)
        deep = a[3].as<bool>();
    if(deep) {
        l = e->eval(p0);
        r = e->eval(p1);
    } else {
        l = p0.clone();
        r = p1.clone();
    }

    if((l.is<double>() || l.is<long>()) && (r.is<double>() || r.is<long>())) {
        bool ret = l.as<double>() != r.as<double>();

        return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
    } else if(l.is<std::string>() && r.is<std::string>()) {
        bool ret = l.as<std::string>() != r.as<std::string>();

        return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
    }
    
    bool ret = !l.equals(r);
    
    return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
}

static Value evalLt(Eval* e, const Arr &a) {
    ValueRef p0 = a[1];
    ValueRef p1 = a[2];
    Value l;
    Value r;
    bool deep = false;
    if(a.count() == 3)
        deep = a[3].as<bool>();
    if(deep) {
        l = e->eval(p0);
        r = e->eval(p1);
    } else {
        l = p0.clone();
        r = p1.clone();
    }

    if((l.is<double>() || l.is<long>()) && (r.is<double>() || r.is<long>())) {
        bool ret = l.as<double>() < r.as<double>();

        return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
    } else if(l.is<std::string>() && r.is<std::string>()) {
        bool ret = l.as<std::string>() < r.as<std::string>();

        return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
    }
    
    bool ret = l.compare(r) < 0;
    
    return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
}

static Value evalLe(Eval* e, const Arr &a) {
    ValueRef p0 = a[1];
    ValueRef p1 = a[2];
    Value l;
    Value r;
    bool deep = false;
    if(a.count() == 3)
        deep = a[3].as<bool>();
    if(deep) {
        l = e->eval(p0);
        r = e->eval(p1);
    } else {
        l = p0.clone();
        r = p1.clone();
    }

    if((l.is<double>() || l.is<long>()) && (r.is<double>() || r.is<long>())) {
        bool ret = l.as<double>() <= r.as<double>();

        return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
    } else if(l.is<std::string>() && r.is<std::string>()) {
        bool ret = l.as<std::string>() <= r.as<std::string>();

        return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
    }
    
    bool ret = l.compare(r) <= 0;
    
    return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
}

static Value evalGt(Eval* e, const Arr &a) {
    ValueRef p0 = a[1];
    ValueRef p1 = a[2];
    Value l;
    Value r;
    bool deep = false;
    if(a.count() == 3)
        deep = a[3].as<bool>();
    if(deep) {
        l = e->eval(p0);
        r = e->eval(p1);
    } else {
        l = p0.clone();
        r = p1.clone();
    }

    if((l.is<double>() || l.is<long>()) && (r.is<double>() || r.is<long>())) {
        bool ret = l.as<double>() > r.as<double>();

        return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
    } else if(l.is<std::string>() && r.is<std::string>()) {
        bool ret = l.as<std::string>() > r.as<std::string>();

        return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
    }
    
    bool ret = l.compare(r) > 0;
    
    return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
}

static Value evalGe(Eval* e, const Arr &a) {
    ValueRef p0 = a[1];
    ValueRef p1 = a[2];
    Value l;
    Value r;
    bool deep = false;
    if(a.count() == 3)
        deep = a[3].as<bool>();
    if(deep) {
        l = e->eval(p0);
        r = e->eval(p1);
    } else {
        l = p0.clone();
        r = p1.clone();
    }

    if((l.is<double>() || l.is<long>()) && (r.is<double>() || r.is<long>())) {
        bool ret = l.as<double>() >= r.as<double>();

        return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
    } else if(l.is<std::string>() && r.is<std::string>()) {
        bool ret = l.as<std::string>() >= r.as<std::string>();

        return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
    }
    
    bool ret = l.compare(r) >= 0;
    
    return Value(DT_DUMMY_DATATREE, NULL, DT_BOOL, ret);
}

unsigned int eval_ver(void) {
    return DTEVER;
}

FuncDef::FuncDef() {
}

FuncDef::FuncDef(const Arr &_args, const Arr &_body) : args(_args), body(_body) {
}

Value FuncDef::operator () (Eval* e, const Arr &a) {
    Value result;

    e->vars->push_back(VarTable());

    for(size_t i = 0; i < args.count(); i++) {
        std::string an = args[i].as<std::string>();
        ValueRef p = a[i + 1];
        e->setVar(an, p.clone());
    }

    for(size_t i = 0; i < body.count(); i++) {
        ValueRef s = body[i];
        result = e->eval(s);
    }

    e->vars->pop_back();

    return result;
}

Invokable::Invokable() : host(NULL) {
}

Invokable::Invokable(const FuncDef &_def) : def(_def), host(NULL) {
}

Invokable::Invokable(const Func &_host) : host(_host) {
}

Value Invokable::operator () (Eval* e, const Arr &a) {
    if(host)
        return host(e, a);
    else
        return def(e, a);
}

Eval::Eval() : funcs(NULL), vars(NULL) {
    _Eval();
}

Eval::Eval(Value &c) : funcs(NULL), vars(NULL) {
    load(c);

    _Eval();
}

Eval::~Eval() {
	delete funcs;
	delete vars;
}

void Eval::_Eval() {
    if(!funcs)
        funcs = new FuncProc;
    if(!vars)
        vars = new VarStack;

    reg("func", evalFunc);
    reg("print", evalPrint);
    reg("if", evalIf);
    reg("while", evalWhile);
    reg("var", evalVar);
    reg("set", evalSet);
    reg("get", evalGet);
    reg("+", evalAdd);
    reg("-", evalMin);
    reg("*", evalMul);
    reg("/", evalDiv);
    reg("%", evalMod);
    reg("==", evalEq);
    reg("!=", evalNotEq);
    reg("<", evalLt);
    reg("<=", evalLe);
    reg(">", evalGt);
    reg(">=", evalGe);
}

void Eval::clear(void) {
    vars->clear();
}

void Eval::reg(const std::string &id, const Func &fn) {
    (*funcs)[id] = fn;
}

void Eval::reg(const std::string &id, const FuncDef &fn) {
    (*funcs)[id] = fn;
}

void Eval::unreg(const std::string &id) {
    FuncProc::iterator it = funcs->find(id);
    if(it != funcs->end())
        funcs->erase(it);
}

void Eval::load(Value &c) {
    code = ValueRef(DT_DUMMY_DATATREE, c.getValue());
}

Value Eval::run(void) {
    Value result;

    vars->push_back(VarTable());

    if(code.is<Arr>()) {
        Arr a = code.as<Arr>();
        for(size_t i = 0; i < a.count(); i++) {
            ValueRef s = a[i];
            result = eval(s);
        }
    }

    vars->pop_back();
    assert(vars->empty());

    return result;
}

Value Eval::setVar(const std::string &id, const Value &v) {
    VarStack::iterator it = vars->end();
    --it;
    --it;
    VarTable &vt = *it;
    Value result = vt[id];
    vt[id] = v;

    return result;
}

ValueRef Eval::getVar(const std::string &id) {
    for(VarStack::reverse_iterator it = vars->rbegin(); it != vars->rend(); ++it) {
        VarTable &vt = *it;
        VarTable::iterator vit = vt.find(id);
        if(vit != vt.end()) {
            ValueRef result;
            result = ValueRef(DT_DUMMY_DATATREE, vit->second.getValue());

            return result;
        }
    }

    return ValueRef();
}

Value Eval::eval(ValueRef &v) {
    Value result;

    vars->push_back(VarTable());

    if(v.is<Arr>()) {
        Arr a = v.as<Arr>();
        ValueRef e = a[0];
        std::string id = e.as<std::string>();
        FuncProc::iterator it = funcs->find(id);
        if(it != funcs->end())
            result = it->second(this, a);
    } else {
        result.cloneFrom(DT_DUMMY_DATATREE, v.getValue());
    }

    vars->pop_back();

    return result;
}

bool Eval::evalBool(ValueRef &v) {
    Value b = eval(v);
    ValueRef br;
    br.setValue(b.getValue());

    return br.to<bool>();
}

}
