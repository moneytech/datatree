#include "dt_eval.hpp"

namespace dt {

static Value evalPrint(Eval* e, const Arr &a) {
    ValueRef t = a[1];
    std::string vn = t.as<std::string>();
    ValueRef v = e->getVar(vn);
    if(v.is<std::string>())
        printf("%s\n", v.as<std::string>().c_str());
    else if(v.is<double>())
        printf("%f\n", v.as<double>());
    else if(v.is<long>())
        printf("%ld\n", v.as<long>());

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

static Value evalAdd(Eval* e, const Arr &a) {
    ValueRef p0 = a[1];
    ValueRef p1 = a[2];
    double r = p0.as<double>() + p1.as<double>();

    return Value(DT_DUMMY_DATATREE, NULL, DT_DOUBLE, r);
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

Eval::Eval(Value &c) {
    code = ValueRef(DT_DUMMY_DATATREE, c.getValue());
	funcs = new FuncProc;
	vars = new VarStack;

    reg("print", evalPrint);
    reg("if", evalIf);
    reg("while", evalWhile);
    reg("add", evalAdd);
    reg("var", evalVar);
}

Eval::~Eval() {
	delete funcs;
	delete vars;
}

void Eval::reg(const std::string &id, Func fn) {
    (*funcs)[id] = fn;
}

void Eval::unreg(const std::string &id) {
    FuncProc::iterator it = funcs->find(id);
    if(it != funcs->end())
        funcs->erase(it);
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
