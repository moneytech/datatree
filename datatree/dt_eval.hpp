#ifndef __DT_EVAL_H__
#define __DT_EVAL_H__

#include <map>
#include <vector>
#include <string>
#include "dt_cpp_helper.hpp"

namespace dt {

struct Eval;

struct FuncDef {
    Arr args;
    Arr body;

    FuncDef();
    FuncDef(const Arr &_args, const Arr &_body);

    Value operator () (Eval* e, const Arr &a);
};

typedef Value (* Func)(Eval*, const Arr &);

struct Invokable {
    FuncDef def;
    Func host;

    Invokable();
    Invokable(const FuncDef &_def);
    Invokable(const Func &_host);

    Value operator () (Eval* e, const Arr &a);
};

typedef std::map<std::string, Invokable> FuncProc;

typedef std::map<std::string, Value> VarTable;

typedef std::vector<VarTable> VarStack;

DT_API unsigned int eval_ver(void);

struct DT_API Eval {
    ValueRef code;
    FuncProc* funcs;
    VarStack* vars;

    Eval();
    Eval(Value &c);
    ~Eval();

    void _Eval();

    void clear(void);

    void reg(const std::string &id, const Func &fn);
    void reg(const std::string &id, const FuncDef &fn);
    void unreg(const std::string &id);

    void load(Value &c);
    Value run(void);

    Value setVar(const std::string &id, const Value &v);
    ValueRef getVar(const std::string &id);

    Value eval(ValueRef &v);
    bool evalBool(ValueRef &v);
};

}

#endif // __DT_EVAL_H__
