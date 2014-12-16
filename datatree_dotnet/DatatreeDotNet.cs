using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace dt
{
    public static class DatatreeDotNet
    {
        public enum dt_bool_t
        {
            DT_FALSE,
            DT_TRUE
        }

        public enum dt_type_t
        {
            DT_NULL,
            DT_BOOL,
            DT_LONG,
            DT_DOUBLE,
            DT_STRING,
            DT_OBJECT,
            DT_ARRAY
        }

        public enum dt_status_t
        {
            DT_OK,
            DT_LOAD_FAILED,
            DT_REDUNDANCE_CHAR_LEFT,
            DT_LEFT_BRACE_EXPECTED,
            DT_RIGHT_BRACE_EXPECTED,
            DT_LEFT_SQUARE_BRACKET_EXPECTED,
            DT_RIGHT_SQUARE_BRACKET_EXPECTED,
            DT_COLON_EXPECTED,
            DT_TOO_MANY_COMMAS,
            DT_VALUE_EXPECTED,
            DT_KEY_EXISTS,
            DT_KEY_DOES_NOT_EXIST,
            DT_INDEX_OUT_OF_RANGE,
            DT_TYPE_NOT_MATCHED,
            DT_VALUE_IS_READONLY,
            DT_JSON_INCOMPATIBLE
        }

        public enum dt_query_status_t
        {
            DTQ_OK,
            DTQ_GOT_REF,
            DTQ_GOT_NOREF,
            DTQ_GOT_NOTHING,
            DTQ_RIGHT_PARENTHESES_EXPECTED,
            DTQ_RIGHT_ANGLE_BRACKET_EXPECTED,
            DTQ_EQUAL_SIGN_EXPECTED,
            DTQ_ARRAY_EXPECTED,
            DTQ_ARRAY_OR_OBJECT_EXPECTED,
            DTQ_PATH_EXPECTED,
            DTQ_CTOR_EXPECTED,
            DTQ_EXPRESSION_EXPECTED
        }

#if DEBUG
        private const string DLL = "datatree_d.dll";
#else
        private const string DLL = "datatree.dll";
#endif

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int dt_separator_char_detect_func_t(byte _c);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void dt_parse_error_handler_t(dt_status_t status, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string msg, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string pos, IntPtr row, IntPtr col);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void dt_object_member_walker_t(IntPtr d, IntPtr o, IntPtr key, IntPtr val, uint idx);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void dt_array_walker_t(IntPtr d, IntPtr a, IntPtr v, uint idx);

        [DllImportAttribute(DLL, EntryPoint = "dt_ver")]
        public static extern uint dt_ver();

        [DllImportAttribute(DLL, EntryPoint = "dt_bver")]
        public static extern uint dt_bver();

        [DllImportAttribute(DLL, EntryPoint = "dt_malloc")]
        public static extern IntPtr dt_malloc([MarshalAsAttribute(UnmanagedType.U4)] uint s);

        [DllImportAttribute(DLL, EntryPoint = "dt_realloc")]
        public static extern IntPtr dt_realloc(ref IntPtr p, [MarshalAsAttribute(UnmanagedType.U4)] uint s);

        [DllImportAttribute(DLL, EntryPoint = "dt_free")]
        public static extern void dt_free(ref IntPtr p);

        [DllImportAttribute(DLL, EntryPoint = "dt_memclr")]
        public static extern void dt_memclr(IntPtr p, [MarshalAsAttribute(UnmanagedType.U4)] uint s);

        [DllImportAttribute(DLL, EntryPoint = "dt_memcmp")]
        public static extern int dt_memcmp(IntPtr l, IntPtr r, [MarshalAsAttribute(UnmanagedType.U4)] uint s);

        [DllImportAttribute(DLL, EntryPoint = "dt_memcpy")]
        public static extern IntPtr dt_memcpy(IntPtr dst, IntPtr src, [MarshalAsAttribute(UnmanagedType.U4)] uint s);

        [DllImportAttribute(DLL, EntryPoint = "dt_memswap")]
        public static extern void dt_memswap(IntPtr l, IntPtr r, [MarshalAsAttribute(UnmanagedType.U4)] uint s);

        [DllImportAttribute(DLL, EntryPoint = "dt_create_datatree")]
        public static extern void dt_create_datatree(ref IntPtr d, dt_parse_error_handler_t eh);

        [DllImportAttribute(DLL, EntryPoint = "dt_unload_datatree")]
        public static extern void dt_unload_datatree(IntPtr d);

        [DllImportAttribute(DLL, EntryPoint = "dt_destroy_datatree")]
        public static extern void dt_destroy_datatree(IntPtr d);

        [DllImportAttribute(DLL, EntryPoint = "dt_get_datatree_userdata")]
        public static extern IntPtr dt_get_datatree_userdata(IntPtr d);

        [DllImportAttribute(DLL, EntryPoint = "dt_set_datatree_userdata")]
        public static extern IntPtr dt_set_datatree_userdata(IntPtr d, IntPtr ud);

        [DllImportAttribute(DLL, EntryPoint = "dt_load_datatree_file")]
        public static extern dt_status_t dt_load_datatree_file(IntPtr d, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string f);

        [DllImportAttribute(DLL, EntryPoint = "dt_save_datatree_file")]
        public static extern dt_status_t dt_save_datatree_file(IntPtr d, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string f, dt_bool_t compact);

        [DllImportAttribute(DLL, EntryPoint = "dt_load_datatree_string")]
        public static extern dt_status_t dt_load_datatree_string(IntPtr d, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string s);

        [DllImportAttribute(DLL, EntryPoint = "dt_save_datatree_string")]
        public static extern dt_status_t dt_save_datatree_string(IntPtr d, ref IntPtr s, dt_bool_t compact);

        [DllImportAttribute(DLL, EntryPoint = "dt_load_datatree_bin")]
        public static extern dt_status_t dt_load_datatree_bin(IntPtr d, IntPtr b);

        [DllImportAttribute(DLL, EntryPoint = "dt_save_datatree_bin")]
        public static extern void dt_save_datatree_bin(IntPtr d, ref IntPtr b, ref uint s);

        #region Create value

        [DllImportAttribute(DLL, EntryPoint = "dt_create_value")]
        public static extern int dt_create_value(IntPtr d, ref IntPtr v, dt_parse_error_handler_t eh, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string fmt);

        [DllImportAttribute(DLL, EntryPoint = "dt_create_value")]
        public static extern int dt_create_value(IntPtr d, ref IntPtr v, dt_parse_error_handler_t eh, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string fmt, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string data);

        [DllImportAttribute(DLL, EntryPoint = "dt_create_value")]
        public static extern int dt_create_value(IntPtr d, ref IntPtr v, dt_parse_error_handler_t eh, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string fmt, dt_type_t t);

        [DllImportAttribute(DLL, EntryPoint = "dt_create_value")]
        public static extern int dt_create_value(IntPtr d, ref IntPtr v, dt_parse_error_handler_t eh, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string fmt, dt_type_t t, dt_bool_t data);

        [DllImportAttribute(DLL, EntryPoint = "dt_create_value")]
        public static extern int dt_create_value(IntPtr d, ref IntPtr v, dt_parse_error_handler_t eh, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string fmt, dt_type_t t, int data);

        [DllImportAttribute(DLL, EntryPoint = "dt_create_value")]
        public static extern int dt_create_value(IntPtr d, ref IntPtr v, dt_parse_error_handler_t eh, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string fmt, dt_type_t t, uint data);

        [DllImportAttribute(DLL, EntryPoint = "dt_create_value")]
        public static extern int dt_create_value(IntPtr d, ref IntPtr v, dt_parse_error_handler_t eh, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string fmt, dt_type_t t, float data);

        [DllImportAttribute(DLL, EntryPoint = "dt_create_value")]
        public static extern int dt_create_value(IntPtr d, ref IntPtr v, dt_parse_error_handler_t eh, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string fmt, dt_type_t t, double data);

        [DllImportAttribute(DLL, EntryPoint = "dt_create_value")]
        public static extern int dt_create_value(IntPtr d, ref IntPtr v, dt_parse_error_handler_t eh, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string fmt, dt_type_t t, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string data);

        [DllImportAttribute(DLL, EntryPoint = "dt_create_value_ex")]
        public static extern int dt_create_value_ex(IntPtr d, ref IntPtr v, dt_separator_char_detect_func_t sd, dt_parse_error_handler_t eh, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string fmt);

        #endregion

        [DllImportAttribute(DLL, EntryPoint = "dt_destroy_value")]
        public static extern void dt_destroy_value(IntPtr d, IntPtr v);

        [DllImportAttribute(DLL, EntryPoint = "dt_format_value")]
        public static extern dt_status_t dt_format_value(IntPtr d, IntPtr v, ref IntPtr s, dt_bool_t compact);

        [DllImportAttribute(DLL, EntryPoint = "dt_clone_value")]
        public static extern void dt_clone_value(IntPtr d, IntPtr v, IntPtr o);

        [DllImportAttribute(DLL, EntryPoint = "dt_find_object_member_by_key")]
        public static extern void dt_find_object_member_by_key(IntPtr d, IntPtr o, IntPtr key, ref IntPtr val, ref uint idx);

        [DllImportAttribute(DLL, EntryPoint = "dt_object_member_count")]
        public static extern void dt_object_member_count(IntPtr d, IntPtr o, ref uint c);

        [DllImportAttribute(DLL, EntryPoint = "dt_object_member_at")]
        public static extern void dt_object_member_at(IntPtr d, IntPtr o, UInt32 idx, ref IntPtr key, ref IntPtr val);

        [DllImportAttribute(DLL, EntryPoint = "dt_array_elem_count")]
        public static extern void dt_array_elem_count(IntPtr d, IntPtr a, ref uint c);

        [DllImportAttribute(DLL, EntryPoint = "dt_array_elem_at")]
        public static extern void dt_array_elem_at(IntPtr d, IntPtr a, [MarshalAsAttribute(UnmanagedType.U4)] uint idx, ref IntPtr v);

        [DllImportAttribute(DLL, EntryPoint = "dt_add_object_member")]
        public static extern dt_status_t dt_add_object_member(IntPtr d, IntPtr o, IntPtr key, IntPtr val, ref uint idx);

        [DllImportAttribute(DLL, EntryPoint = "dt_insert_object_member")]
        public static extern dt_status_t dt_insert_object_member(IntPtr d, IntPtr o, UInt32 @where, IntPtr key, IntPtr val);

        [DllImportAttribute(DLL, EntryPoint = "dt_add_array_elem")]
        public static extern dt_status_t dt_add_array_elem(IntPtr d, IntPtr a, IntPtr v, ref uint idx);

        [DllImportAttribute(DLL, EntryPoint = "dt_insert_array_elem")]
        public static extern dt_status_t dt_insert_array_elem(IntPtr d, IntPtr a, UInt32 @where, IntPtr v);

        [DllImportAttribute(DLL, EntryPoint = "dt_remove_object_member_by_key")]
        public static extern dt_status_t dt_remove_object_member_by_key(IntPtr d, IntPtr o, IntPtr key, IntPtr val);

        [DllImportAttribute(DLL, EntryPoint = "dt_delete_object_member_by_key")]
        public static extern dt_status_t dt_delete_object_member_by_key(IntPtr d, IntPtr o, IntPtr key);

        [DllImportAttribute(DLL, EntryPoint = "dt_remove_object_member_at")]
        public static extern dt_status_t dt_remove_object_member_at(IntPtr d, IntPtr o, [MarshalAsAttribute(UnmanagedType.U4)] uint @where, IntPtr key, IntPtr val);

        [DllImportAttribute(DLL, EntryPoint = "dt_delete_object_member_at")]
        public static extern dt_status_t dt_delete_object_member_at(IntPtr d, IntPtr o, UInt32 @where);

        [DllImportAttribute(DLL, EntryPoint = "dt_clear_object_member")]
        public static extern dt_status_t dt_clear_object_member(IntPtr d, IntPtr o);

        [DllImportAttribute(DLL, EntryPoint = "dt_remove_array_elem_at")]
        public static extern dt_status_t dt_remove_array_elem_at(IntPtr d, IntPtr a, [MarshalAsAttribute(UnmanagedType.U4)] uint @where, IntPtr val);

        [DllImportAttribute(DLL, EntryPoint = "dt_delete_array_elem_at")]
        public static extern dt_status_t dt_delete_array_elem_at(IntPtr d, IntPtr a, UInt32 @where);

        [DllImportAttribute(DLL, EntryPoint = "dt_clear_array_elem")]
        public static extern dt_status_t dt_clear_array_elem(IntPtr d, IntPtr a);

        [DllImportAttribute(DLL, EntryPoint = "dt_update_object_member_by_key")]
        public static extern dt_status_t dt_update_object_member_by_key(IntPtr d, IntPtr o, IntPtr key, IntPtr val);

        [DllImportAttribute(DLL, EntryPoint = "dt_update_object_member_at")]
        public static extern dt_status_t dt_update_object_member_at(IntPtr d, IntPtr o, [MarshalAsAttribute(UnmanagedType.U4)] uint @where, IntPtr val);

        [DllImportAttribute(DLL, EntryPoint = "dt_udpate_array_elem_at")]
        public static extern dt_status_t dt_udpate_array_elem_at(IntPtr d, IntPtr a, [MarshalAsAttribute(UnmanagedType.U4)] uint @where, IntPtr val);

        [DllImportAttribute(DLL, EntryPoint = "dt_foreach_object_member")]
        public static extern void dt_foreach_object_member(IntPtr d, IntPtr o, dt_object_member_walker_t w);

        [DllImportAttribute(DLL, EntryPoint = "dt_foreach_array_elem")]
        public static extern void dt_foreach_array_elem(IntPtr d, IntPtr a, dt_array_walker_t w);

        [DllImportAttribute(DLL, EntryPoint = "dt_root_value")]
        public static extern IntPtr dt_root_value(IntPtr d);

        [DllImportAttribute(DLL, EntryPoint = "dt_root_as_object")]
        public static extern IntPtr dt_root_as_object(IntPtr d);

        [DllImportAttribute(DLL, EntryPoint = "dt_value_type")]
        public static extern dt_type_t dt_value_type(IntPtr v);

        [DllImportAttribute(DLL, EntryPoint = "dt_value_data")]
        public static extern IntPtr dt_value_data(IntPtr v);

        [DllImportAttribute(DLL, EntryPoint = "dt_value_data_as")]
        public static extern void dt_value_data_as(IntPtr des, IntPtr v, dt_type_t t);

        [DllImportAttribute(DLL, EntryPoint = "dt_value_compare")]
        public static extern int dt_value_compare(IntPtr l, IntPtr r, dt_bool_t num_raw_cmp);

        [DllImportAttribute(DLL, EntryPoint = "dt_value_data_length")]
        public static extern int dt_value_data_length(IntPtr v);

        [DllImportAttribute(DLL, EntryPoint = "dt_value_mem_swap")]
        public static extern void dt_value_mem_swap(IntPtr l, IntPtr r);

        [DllImportAttribute(DLL, EntryPoint = "dt_value_mem_move")]
        public static extern void dt_value_mem_move(IntPtr l, IntPtr r);

        [DllImportAttribute(DLL, EntryPoint = "dt_qver")]
        public static extern uint dt_qver();

        [DllImportAttribute(DLL, EntryPoint = "dt_query_status_message")]
        [return: MarshalAs(UnmanagedType.LPStr)] 
        public static extern string dt_query_status_message(dt_query_status_t s);

        [DllImportAttribute(DLL, EntryPoint = "dt_create_command")]
        public static extern void dt_create_command(ref IntPtr c);

        [DllImportAttribute(DLL, EntryPoint = "dt_destroy_command")]
        public static extern void dt_destroy_command(IntPtr c);

        [DllImportAttribute(DLL, EntryPoint = "dt_parse_command")]
        public static extern int dt_parse_command(IntPtr c, dt_parse_error_handler_t eh, [InAttribute()] [MarshalAsAttribute(UnmanagedType.LPStr)] string cmd);

        [DllImportAttribute(DLL, EntryPoint = "dt_clear_command")]
        public static extern void dt_clear_command(IntPtr c);

        [DllImportAttribute(DLL, EntryPoint = "dt_query")]
        public static extern dt_query_status_t dt_query(IntPtr t, IntPtr c, ref IntPtr ret);

        [DllImportAttribute(DLL, EntryPoint = "dt_last_queried")]
        public static extern IntPtr dt_last_queried(IntPtr c);
    }
}
