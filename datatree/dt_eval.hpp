#ifndef __DT_EVAL_H__
#define __DT_EVAL_H__

#include <map>
#include <vector>
#include <string>
#include "dt_cpp_helper.hpp"

namespace dt {

struct Eval;

typedef Value (* Func)(Eval*, const Arr &);

typedef std::map<std::string, Func> FuncProc;

typedef std::map<std::string, Value> VarTable;

typedef std::vector<VarTable> VarStack;

struct DT_API Eval {
    ValueRef code;
    FuncProc* funcs;
    VarStack* vars;

    Eval(Value &c);
    ~Eval();

    void reg(const std::string &id, Func fn);
    void unreg(const std::string &id);

    Value run(void);

    Value setVar(const std::string &id, const Value &v);
    ValueRef getVar(const std::string &id);

    Value eval(ValueRef &v);
    bool evalBool(ValueRef &v);
};

}

#endif // __DT_EVAL_H__
