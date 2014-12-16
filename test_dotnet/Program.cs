using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using dt;

namespace test_dotnet
{
    class Program
    {
        static void _walk_object_members(IntPtr d, IntPtr o, IntPtr key, IntPtr val, uint idx)
        {
        }

        static void _walk_array_elements(IntPtr d, IntPtr a, IntPtr v, uint idx)
        {
        }

        static void _on_parse_error(DatatreeDotNet.dt_status_t status, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string msg, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string pos, System.IntPtr row, System.IntPtr col)
        {
            Debug.Print("Parsing error.\nError code: {0}, error message: {1}\nRow: {2}, col: {3}\nText: {4}\n", status, msg, row, col, pos);
        }

        static void Main(string[] args)
        {
            IntPtr data_tree = IntPtr.Zero;
            IntPtr v0 = IntPtr.Zero;
            IntPtr v1 = IntPtr.Zero;
            IntPtr iv = IntPtr.Zero;
            IntPtr a = IntPtr.Zero;
            uint idx = 0;
            IntPtr fmt = IntPtr.Zero;
            IntPtr bin = IntPtr.Zero;
            uint sz = 0;

            DatatreeDotNet.dt_create_datatree(ref data_tree, _on_parse_error);

            // common test
            DatatreeDotNet.dt_load_datatree_string(data_tree, "'dt test'\n{\n  \"hello\" : \"world\",\n  \"data_tree\" : \"test\"\n}");

            // object key test
            DatatreeDotNet.dt_load_datatree_string(data_tree, "{ { \"hello\" : \"world\" } : 123 }");

            // array test
            DatatreeDotNet.dt_load_datatree_string(data_tree, "{ [\"hello\", \"world\"] : [123, 3.14] }");

            // multy types test
            DatatreeDotNet.dt_load_datatree_string(data_tree, "{ null : [2123, 3.1415926535897932384626, \"test\"], true : false, name : paladin_t }");

            // search test
            DatatreeDotNet.dt_create_value(data_tree, ref v0, _on_parse_error, "a_%s_b", "and");
            DatatreeDotNet.dt_destroy_value(data_tree, v0);
            DatatreeDotNet.dt_create_value(data_tree, ref v0, _on_parse_error, null, DatatreeDotNet.dt_type_t.DT_STRING, "name");
            DatatreeDotNet.dt_find_object_member_by_key(data_tree, DatatreeDotNet.dt_root_as_object(data_tree), v0, ref iv, ref idx);
            DatatreeDotNet.dt_destroy_value(data_tree, v0);

            // insert test
            DatatreeDotNet.dt_create_value(data_tree, ref v0, _on_parse_error, null, DatatreeDotNet.dt_type_t.DT_LONG, 123456);
            DatatreeDotNet.dt_create_value(data_tree, ref v1, _on_parse_error, null, DatatreeDotNet.dt_type_t.DT_STRING, "value test");
            DatatreeDotNet.dt_insert_object_member(data_tree, DatatreeDotNet.dt_root_as_object(data_tree), 1, v0, v1);
            DatatreeDotNet.dt_destroy_value(data_tree, v0);
            DatatreeDotNet.dt_destroy_value(data_tree, v1);

            DatatreeDotNet.dt_object_member_at(data_tree, DatatreeDotNet.dt_root_as_object(data_tree), 0, ref v0, ref v1);
            a = DatatreeDotNet.dt_value_data(v1);
            DatatreeDotNet.dt_create_value(data_tree, ref v0, _on_parse_error, null, DatatreeDotNet.dt_type_t.DT_STRING, "inserted");
            DatatreeDotNet.dt_insert_array_elem(data_tree, a, 1, v0);
            DatatreeDotNet.dt_destroy_value(data_tree, v0);

            // delete test
            DatatreeDotNet.dt_delete_array_elem_at(data_tree, a, 1);
            DatatreeDotNet.dt_delete_object_member_at(data_tree, DatatreeDotNet.dt_root_as_object(data_tree), 2);

            // update test

            // foreach test
            DatatreeDotNet.dt_foreach_object_member(data_tree, DatatreeDotNet.dt_root_as_object(data_tree), _walk_object_members);
            DatatreeDotNet.dt_foreach_array_elem(data_tree, a, _walk_array_elements);

            // clone test
            DatatreeDotNet.dt_create_value(data_tree, ref v0, _on_parse_error, null, null);
            DatatreeDotNet.dt_clone_value(data_tree, DatatreeDotNet.dt_root_value(data_tree), v0);
            DatatreeDotNet.dt_format_value(data_tree, v0, ref fmt, DatatreeDotNet.dt_bool_t.DT_FALSE);
            string str = Marshal.PtrToStringAnsi(fmt);
            Debug.Print("cloned value:\n{0}\n", str);
            DatatreeDotNet.dt_free(ref fmt);
            DatatreeDotNet.dt_destroy_value(data_tree, v0);

            // serialize test
            DatatreeDotNet.dt_save_datatree_bin(data_tree, ref bin, ref sz);
            DatatreeDotNet.dt_load_datatree_bin(data_tree, bin);
            DatatreeDotNet.dt_free(ref bin);

            // format test
            DatatreeDotNet.dt_save_datatree_string(data_tree, ref fmt, DatatreeDotNet.dt_bool_t.DT_FALSE);
            str = Marshal.PtrToStringAnsi(fmt);
            Debug.Print("root value:\n{0}\n", str);
            DatatreeDotNet.dt_free(ref fmt);

            // cpp helper test
            //dt::Value vp0(data_tree, null, DT_SHORT, 123);
            //dt::Value vp1(vp0);
            //dt::Value vp2;
            //vp2.cloneFrom(vp1);
            //vp2.cloneFrom(data_tree, dt_root_value(data_tree));
            //std::string vpstr = vp2.toString();
            //int cmp = vp2.compare(vp1);
            //cmp = vp2.compare(dt_root_value(data_tree));

            DatatreeDotNet.dt_unload_datatree(data_tree);
            DatatreeDotNet.dt_destroy_datatree(data_tree);
        }
    }
}
