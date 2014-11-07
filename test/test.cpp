#include "../datatree/dt_core.h"
#include "../datatree/dt_query.h"
#include "../datatree/dt_cpp_helper.hpp"
#include "../datatree/dt_eval.hpp"

static void _ensure(bool cond, const char* msg) {
	if(!cond) {
		printf("%s\n", msg);
		abort();
	}
}

static void _on_simple_parsed(dt_type_t type, void* data, dt_sad_handler_t* sad) {
	(void)type;
	(void)data;
	(void)sad;
}

static void _on_object_begin(dt_sad_handler_t* sad) {
	(void)sad;
}

static void _on_object_end(dt_sad_handler_t* sad) {
	(void)sad;
}

static void _on_array_begin(dt_sad_handler_t* sad) {
	(void)sad;
}

static void _on_array_end(dt_sad_handler_t* sad) {
	(void)sad;
}

static void _walk_object_members(dt_datatree_t d, const dt_object_t o, const dt_value_t key, const dt_value_t val, size_t idx) {
	(void)d;
	(void)o;
	(void)key;
	(void)val;
	(void)idx;
}

static void _walk_array_elements(dt_datatree_t d, const dt_array_t a, const dt_value_t v, size_t idx) {
	(void)d;
	(void)a;
	(void)v;
	(void)idx;
}

static void _on_parse_error(dt_enum_compatible_t status, const char* msg, const char* pos, size_t row, size_t col) {
	printf("Parsing error.\nError code: %d, error message: %s\nRow: %d, col: %d\nText: %s...\n", status, msg, row, col, pos);
}

static void _on_exit(void) {
#ifdef DT_ENABLE_ALLOC_STAT
	size_t mem_stat = dt_allocated();
	DT_ASSERT(mem_stat == 0);
#endif /* DT_ENABLE_ALLOC_STAT */

#ifdef _MSC_VER
	int c = _CrtDumpMemoryLeaks();

	if(0 != c) {
#ifdef _DEBUG
		_asm int 3
#else
		_asm int 3
#endif
	}
#endif /* _MSC_VER */
}

int main(int argc, char* argv[]) {
#ifdef _MSC_VER
	_CrtSetBreakAlloc(0);
#endif /* _MSC_VER */

	atexit(_on_exit);

	{
		dt_datatree_t d = NULL;
		dt_value_t v0 = NULL;
		dt_value_t v1 = NULL;
		dt_value_t iv = NULL;
		dt_array_t a = NULL;
		size_t idx = 0;
		char* fmt = NULL;
		void* bin = NULL;
		size_t sz = 0;
		size_t mem_stat = 0; (void)mem_stat;
		dt_sad_handler_t sad_handler = {
			_on_simple_parsed,
			_on_object_begin,
			_on_object_end,
			_on_array_begin,
			_on_array_end,
			NULL
		};

		unsigned int ver = dt_ver();
		printf("datatree version: 0x%08x\n", ver);

		dt::Value dts(
			DT_DUMMY_DATATREE,
			"[[var, a, 2],\n[print, a]]"
		);
		dt::Eval eval(dts);
		eval.run();

		dt_create_datatree(&d, _on_parse_error);
			// SAD test
			dt_load_datatree_string_sad(d, &sad_handler, "{ [\"useful\", \"sad\"] : null }");

			// common test
			dt_load_datatree_string(d, "'dt test'\n{\n  \"hello\" : \"world\",\n  \"data_tree\" : \"test\"\n}");

#ifdef DT_ENABLE_ALLOC_STAT
			mem_stat = dt_allocated();
			printf("Allocated memory: %u bytes\n", mem_stat);
#endif /* DT_ENABLE_ALLOC_STAT */

			// object key test
			dt_load_datatree_string(d, "{ { \"hello\" : \"world\" } : 123 }");

			// array test
			dt_load_datatree_string(d, "{ [\"hello\", \"world\"] : [123, 3.14] }");

			// multy types test
			dt_load_datatree_string(d, "{ null : [ { id : 1, data : one }, { id : 2, data : two }, { id : 3, data : three }, 2123, [3.1415926535897932384626, in_array], \"test\"], true : false, \"name\" : paladin_t }");

			// search test
			dt_create_value(d, &v0, _on_parse_error, "a_%s_b", "and");
			dt_destroy_value(d, v0);
			dt_create_value(d, &v0, _on_parse_error, NULL, DT_STRING, "name");
			dt_find_object_member_by_key(d, dt_root_as_object(d), v0, &iv, &idx);
			dt_destroy_value(d, v0);

			// insert test
			dt_create_value(d, &v0, _on_parse_error, NULL, DT_LONG, 123456);
			dt_create_value(d, &v1, _on_parse_error, NULL, DT_STRING, "value test");
			dt_insert_object_member(d, dt_root_as_object(d), 1, v0, v1);
			dt_destroy_value(d, v0);
			dt_destroy_value(d, v1);

			dt_object_member_at(d, dt_root_as_object(d), 0, &v0, &v1);
			a = (dt_array_t)dt_value_data(v1);
			dt_create_value(d, &v0, _on_parse_error, NULL, DT_STRING, "inserted");
			dt_insert_array_elem(d, a, 1, v0);
			dt_destroy_value(d, v0);

			// delete test
			dt_delete_array_elem_at(d, a, 1);
			dt_delete_object_member_at(d, dt_root_as_object(d), 2);

			// update test

			// foreach test
			dt_foreach_object_member(d, dt_root_as_object(d), _walk_object_members);
			dt_foreach_array_elem(d, a, _walk_array_elements);

			// clone test
			dt_create_value(d, &v0, _on_parse_error, NULL, DT_NULL);
			dt_clone_value(d, dt_root_value(d), v0);
			dt_format_value(d, v0, &fmt, DT_FALSE);
			printf("cloned value:\n%s\n", fmt);
			dt_free((void**)&fmt);
			fmt = NULL;
			dt_destroy_value(d, v0);

			// serialize test
			dt_save_datatree_bin(d, &bin, &sz);
			dt_load_datatree_bin(d, bin);
			dt_free(&bin);

			// format test
			dt_save_datatree_string(d, &fmt, DT_FALSE);
			printf("root value:\n%s\n", fmt);
			dt_free((void**)&fmt);
			fmt = NULL;

			// cpp helper test
			dt::Value vp0(d, NULL, DT_LONG, 123);
			dt::Value vp1(vp0);
			dt::Value vp2;
			vp2.cloneFrom(vp1);
			vp2.cloneFrom(d, dt_root_value(d));
			std::string vpstr = vp2.toString();
			int cmp = vp2.compare(vp1);
			cmp = vp2.compare(dt_root_value(d));

			// path test
			dt::Command cmd0("select .null<1><1>");
			dt::Value ret0 = cmd0.query(dt_root_value(d));
			printf("query result: %s\n", ret0.toString().c_str());
			dt::Value ensure = dt::Value(DT_DUMMY_DATATREE, "two");
			_ensure(ensure.equals(ret0), "Error result");

			// query test
			dt::Command cmd1("select ID = .null<1><0>, DATA = .null<1>.data");
			dt::Value ret1 = cmd1.query(dt_root_value(d));
			printf("query result: %s\n", ret1.toString().c_str());
			ensure = dt::Value(DT_DUMMY_DATATREE, "{ID:2,DATA:two}");
			_ensure(ensure.equals(ret1), "Error result");

			dt::Command cmd2("from .null select ID = <0>, DATA = .data orderby .DATA");
			dt::Value ret2 = cmd2.query(dt_root_value(d));
			printf("query result: %s\n", ret2.toString().c_str());
			ensure = dt::Value(DT_DUMMY_DATATREE, "[{ID:1,DATA:one},{ID:3,DATA:three},{ID:2,DATA:two}]");
			_ensure(ensure.equals(ret2), "Error result");

			dt::Command cmd3("from from .null select ID = <0>, DATA = .data select .ID");
			dt::Value ret3 = cmd3.query(dt_root_value(d));
			printf("query result: %s\n", ret3.toString().c_str());
			ensure = dt::Value(DT_DUMMY_DATATREE, "[1,2,3]");
			_ensure(ensure.equals(ret3), "Error result");

			dt::Command cmd4("select .null._len_");
			dt::Value ret4 = cmd4.query(dt_root_value(d));
			printf("query result: %s\n", ret4.toString().c_str());
			_ensure(ret4.as<int>() == 6, "Error result");

			dt::Command cmd5("from .null select ._this_");
			dt::Value ret5 = cmd5.query(dt_root_value(d));
			printf("query result: %s\n", ret5.toString().c_str());
			ensure = dt::Value(DT_DUMMY_DATATREE, "[{id:1,data:one},{id:2,data:two},{id:3,data:three},2123,[3.1415926535897932384626,in_array],test]");
			_ensure(ensure.equals(ret5), "Error result");

			dt::Command cmd6("from .null where .id ge 2 or .id lt 2 select ID = <0>, DATA = .data");
			dt::Value ret6 = cmd6.query(dt_root_value(d));
			printf("query result: %s\n", ret6.toString().c_str());
			ensure = dt::Value(DT_DUMMY_DATATREE, "[{ID:1,DATA:one},{ID:2,DATA:two},{ID:3,DATA:three}]");
			_ensure(ensure.equals(ret6), "Error result");

#ifdef DT_ENABLE_ALLOC_STAT
			mem_stat = dt_allocated();
			printf("Allocated memory: %u bytes\n", mem_stat);
#endif /* DT_ENABLE_ALLOC_STAT */

			dt_unload_datatree(d);
		dt_destroy_datatree(d);
	}

	return 0;
}
