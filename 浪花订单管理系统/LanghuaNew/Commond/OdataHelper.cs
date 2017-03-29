using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commond
{
   public class OdataHelper
    {
        private string Value = "";
        public  OdataHelper(string left,string right, ExpressionType Operate)
        {
            Value = SetString(left,right,Operate);
        }
        public OdataHelper(string left, int right, ExpressionType Operate)
        {
            Value = SetString(left, right, Operate);
        }
        public OdataHelper(string left, DateTime right, ExpressionType Operate)
        {
            Value = SetString(left, right, Operate);
        }
        public void Or(string left, string right, ExpressionType Operate)
        {
            Value += string.Format(" {0} {1}", GetOperateName(ExpressionType.Or), SetString(left, right, Operate));
        }

        public void And(string left, string right, ExpressionType Operate)
        {
            Value += string.Format(" {0} {1}", GetOperateName(ExpressionType.And), SetString(left, right, Operate));
        }

        private string SetString(string left, string right, ExpressionType Operate)
        {
            return string.Format(" {0} {1} '{2}'", left, GetOperateName(Operate), right);
        }
        //
        public void Or(string left, int right, ExpressionType Operate)
        {
            Value += string.Format(" {0} {1}", GetOperateName(ExpressionType.Or), SetString(left, right, Operate));
        }

        public void And(string left, int right, ExpressionType Operate)
        {
            Value += string.Format(" {0} {1}", GetOperateName(ExpressionType.And), SetString(left, right, Operate));
        }

        private string SetString(string left, int right, ExpressionType Operate)
        {
            return string.Format(" {0} {1} {2}", left, GetOperateName(Operate), right);
        }

        public void Or(string left, DateTime right, ExpressionType Operate)
        {
            Value += string.Format(" {0} {1}", GetOperateName(ExpressionType.Or), SetString(left, right, Operate));
        }

        public void And(string left, DateTime right, ExpressionType Operate)
        {
            Value += string.Format(" {0} {1}", GetOperateName(ExpressionType.And), SetString(left, right, Operate));
        }

        private string SetString(string left, DateTime right, ExpressionType Operate)
        {
            return string.Format(" {0} {1} {2}", left, GetOperateName(Operate), right.ToString("yyyy-MM-dd"));
        }

        public string GetValue()
        {
            return Value;
        }



        private string GetOperateName(ExpressionType Operate)
        {
            switch (Operate)
            {
                case ExpressionType.And:
                    return  "and";
                case ExpressionType.Or:
                    return "or";
                case ExpressionType.Not:
                    return "not";
                case ExpressionType.Equal:
                    return  "eq";
                case ExpressionType.NotEqual:
                    return "ne";
                case ExpressionType.GreaterThan:
                    return "gt";
                case ExpressionType.GreaterThanOrEqual:
                    return "ge";
                case ExpressionType.LessThan:
                    return "lt";
                case ExpressionType.LessThanOrEqual:
                    return "le";
                case ExpressionType.Add:
                    return "add";
                case ExpressionType.Subtract:
                    return "sub";
                case ExpressionType.Multiply:
                    return "mul";
                case ExpressionType.Divide:
                    return "div";
                case ExpressionType.Modulo:
                    return "mod";
                case ExpressionType.Negate:
                    return "-";
                default:
                    return null;
            }
        }
    }

    public enum ExpressionType
    {
        //
        // 摘要:
        //     加法运算，如 a + b，针对数值操作数，不进行溢出检查。
        Add = 0,
        //
        // 摘要:
        //     加法运算，如 (a + b)，针对数值操作数，进行溢出检查。
        AddChecked = 1,
        //
        // 摘要:
        //     按位或逻辑 AND 运算，如 C# 中的 (a & b) 和 Visual Basic 中的 (a And b)。
        And = 2,
        //
        // 摘要:
        //     条件 AND 运算，它仅在第一个操作数的计算结果为 true 时才计算第二个操作数。它与 C# 中的 (a && b) 和 Visual Basic 中的
        //     (a AndAlso b) 对应。
        AndAlso = 3,
        //
        // 摘要:
        //     获取一维数组长度的运算，如 array.Length。
        ArrayLength = 4,
        //
        // 摘要:
        //     一维数组中的索引运算，如 C# 中的 array[index] 或 Visual Basic 中的 array(index)。
        ArrayIndex = 5,
        //
        // 摘要:
        //     方法调用，如在 obj.sampleMethod() 表达式中。
        Call = 6,
        //
        // 摘要:
        //     表示 null 合并运算的节点，如 C# 中的 (a ?? b) 或 Visual Basic 中的 If(a, b)。
        Coalesce = 7,
        //
        // 摘要:
        //     条件运算，如 C# 中的 a > b ? a : b 或 Visual Basic 中的 If(a > b, a, b)。
        Conditional = 8,
        //
        // 摘要:
        //     一个常量值。
        Constant = 9,
        //
        // 摘要:
        //     强制转换或转换运算，如 C# 中的 (SampleType)obj 或 Visual Basic 中的 CType(obj, SampleType)。对于数值转换，如果转换后的值对于目标类型来说太大，这不会引发异常。
        Convert = 10,
        //
        // 摘要:
        //     强制转换或转换运算，如 C# 中的 (SampleType)obj 或 Visual Basic 中的 CType(obj, SampleType)。对于数值转换，如果转换后的值与目标类型大小不符，则引发异常。
        ConvertChecked = 11,
        //
        // 摘要:
        //     除法运算，如 (a / b)，针对数值操作数。
        Divide = 12,
        //
        // 摘要:
        //     表示相等比较的节点，如 C# 中的 (a == b) 或 Visual Basic 中的 (a = b)。
        Equal = 13,
        //
        // 摘要:
        //     按位或逻辑 XOR 运算，如 C# 中的 (a ^ b) 或 Visual Basic 中的 (a Xor b)。
        ExclusiveOr = 14,
        //
        // 摘要:
        //     “大于”比较，如 (a > b)。
        GreaterThan = 15,
        //
        // 摘要:
        //     “大于或等于”比较，如 (a >= b)。
        GreaterThanOrEqual = 16,
        //
        // 摘要:
        //     调用委托或 lambda 表达式的运算，如 sampleDelegate.Invoke()。
        Invoke = 17,
        //
        // 摘要:
        //     lambda 表达式，如 C# 中的 a => a + a 或 Visual Basic 中的 Function(a) a + a。
        Lambda = 18,
        //
        // 摘要:
        //     按位左移运算，如 (a << b)。
        LeftShift = 19,
        //
        // 摘要:
        //     “小于”比较，如 (a < b)。
        LessThan = 20,
        //
        // 摘要:
        //     “小于或等于”比较，如 (a <= b)。
        LessThanOrEqual = 21,
        //
        // 摘要:
        //     创建新的 System.Collections.IEnumerable 对象并从元素列表中初始化该对象的运算，如 C# 中的 new List<SampleType>(){
        //     a, b, c } 或 Visual Basic 中的 Dim sampleList = { a, b, c }。
        ListInit = 22,
        //
        // 摘要:
        //     从字段或属性进行读取的运算，如 obj.SampleProperty。
        MemberAccess = 23,
        //
        // 摘要:
        //     创建新的对象并初始化其一个或多个成员的运算，如 C# 中的 new Point { X = 1, Y = 2 } 或 Visual Basic 中的 New
        //     Point With {.X = 1, .Y = 2}。
        MemberInit = 24,
        //
        // 摘要:
        //     算术余数运算，如 C# 中的 (a % b) 或 Visual Basic 中的 (a Mod b)。
        Modulo = 25,
        //
        // 摘要:
        //     乘法运算，如 (a * b)，针对数值操作数，不进行溢出检查。
        Multiply = 26,
        //
        // 摘要:
        //     乘法运算，如 (a * b)，针对数值操作数，进行溢出检查。
        MultiplyChecked = 27,
        //
        // 摘要:
        //     算术求反运算，如 (-a)。不应就地修改 a 对象。
        Negate = 28,
        //
        // 摘要:
        //     一元加法运算，如 (+a)。预定义的一元加法运算的结果是操作数的值，但用户定义的实现可以产生特殊结果。
        UnaryPlus = 29,
        //
        // 摘要:
        //     算术求反运算，如 (-a)，进行溢出检查。不应就地修改 a 对象。
        NegateChecked = 30,
        //
        // 摘要:
        //     调用构造函数创建新对象的运算，如 new SampleType()。
        New = 31,
        //
        // 摘要:
        //     创建新的一维数组并从元素列表中初始化该数组的运算，如 C# 中的 new SampleType[]{a, b, c} 或 Visual Basic 中的
        //     New SampleType(){a, b, c}。
        NewArrayInit = 32,
        //
        // 摘要:
        //     创建新数组（其中每个维度的界限均已指定）的运算，如 C# 中的 new SampleType[dim1, dim2] 或 Visual Basic 中的
        //     New SampleType(dim1, dim2)。
        NewArrayBounds = 33,
        //
        // 摘要:
        //     按位求补运算或逻辑求反运算。在 C# 中，它与整型的 (~a) 和布尔值的 (!a) 等效。在 Visual Basic 中，它与 (Not a) 等效。不应就地修改
        //     a 对象。
        Not = 34,
        //
        // 摘要:
        //     不相等比较，如 C# 中的 (a != b) 或 Visual Basic 中的 (a <> b)。
        NotEqual = 35,
        //
        // 摘要:
        //     按位或逻辑 OR 运算，如 C# 中的 (a | b) 或 Visual Basic 中的 (a Or b)。
        Or = 36,
        //
        // 摘要:
        //     短路条件 OR 运算，如 C# 中的 (a || b) 或 Visual Basic 中的 (a OrElse b)。
        OrElse = 37,
        //
        // 摘要:
        //     对在表达式上下文中定义的参数或变量的引用。有关更多信息，请参见 System.Linq.Expressions.ParameterExpression。
        Parameter = 38,
        //
        // 摘要:
        //     对某个数字进行幂运算的数学运算，如 Visual Basic 中的 (a ^ b)。
        Power = 39,
        //
        // 摘要:
        //     具有类型为 System.Linq.Expressions.Expression 的常量值的表达式。System.Linq.Expressions.ExpressionType.Quote
        //     节点可包含对参数的引用，这些参数在该节点表示的表达式的上下文中定义。
        Quote = 40,
        //
        // 摘要:
        //     按位右移运算，如 (a >> b)。
        RightShift = 41,
        //
        // 摘要:
        //     减法运算，如 (a - b)，针对数值操作数，不进行溢出检查。
        Subtract = 42,
        //
        // 摘要:
        //     算术减法运算，如 (a - b)，针对数值操作数，进行溢出检查。
        SubtractChecked = 43,
        //
        // 摘要:
        //     显式引用或装箱转换，其中如果转换失败则提供 null，如 C# 中的 (obj as SampleType) 或 Visual Basic 中的 TryCast(obj,
        //     SampleType)。
        TypeAs = 44,
        //
        // 摘要:
        //     类型测试，如 C# 中的 obj is SampleType 或 Visual Basic 中的 TypeOf obj is SampleType。
        TypeIs = 45,
        //
        // 摘要:
        //     赋值运算，如 (a = b)。
        Assign = 46,
        //
        // 摘要:
        //     表达式块。
        Block = 47,
        //
        // 摘要:
        //     调试信息。
        DebugInfo = 48,
        //
        // 摘要:
        //     一元递减运算，如 C# 和 Visual Basic 中的 (a - 1)。不应就地修改 a 对象。
        Decrement = 49,
        //
        // 摘要:
        //     动态操作。
        Dynamic = 50,
        //
        // 摘要:
        //     默认值。
        Default = 51,
        //
        // 摘要:
        //     扩展表达式。
        Extension = 52,
        //
        // 摘要:
        //     “跳转”表达式，如 C# 中的 goto Label 或 Visual Basic 中的 GoTo Label。
        Goto = 53,
        //
        // 摘要:
        //     一元递增运算，如 C# 和 Visual Basic 中的 (a + 1)。不应就地修改 a 对象。
        Increment = 54,
        //
        // 摘要:
        //     索引运算或访问使用参数的属性的运算。
        Index = 55,
        //
        // 摘要:
        //     标签。
        Label = 56,
        //
        // 摘要:
        //     运行时变量的列表。有关更多信息，请参见 System.Linq.Expressions.RuntimeVariablesExpression。
        RuntimeVariables = 57,
        //
        // 摘要:
        //     循环，如 for 或 while。
        Loop = 58,
        //
        // 摘要:
        //     多分支选择运算，如 C# 中的 switch 或 Visual Basic 中的 Select Case。
        Switch = 59,
        //
        // 摘要:
        //     引发异常的运算，如 throw new Exception()。
        Throw = 60,
        //
        // 摘要:
        //     try-catch 表达式。
        Try = 61,
        //
        // 摘要:
        //     取消装箱值类型运算，如 MSIL 中的 unbox 和 unbox.any 指令。
        Unbox = 62,
        //
        // 摘要:
        //     加法复合赋值运算，如 (a += b)，针对数值操作数，不进行溢出检查。
        AddAssign = 63,
        //
        // 摘要:
        //     按位或逻辑 AND 复合赋值运算，如 C# 中的 (a &= b)。
        AndAssign = 64,
        //
        // 摘要:
        //     除法复合赋值运算，如 (a /= b)，针对数值操作数。
        DivideAssign = 65,
        //
        // 摘要:
        //     按位或逻辑 XOR 复合赋值运算，如 C# 中的 (a ^= b)。
        ExclusiveOrAssign = 66,
        //
        // 摘要:
        //     按位左移复合赋值运算，如 (a <<= b)。
        LeftShiftAssign = 67,
        //
        // 摘要:
        //     算术余数复合赋值运算，如 C# 中的 (a %= b)。
        ModuloAssign = 68,
        //
        // 摘要:
        //     乘法复合赋值运算，如 (a *= b)，针对数值操作数，不进行溢出检查。
        MultiplyAssign = 69,
        //
        // 摘要:
        //     按位或逻辑 OR 复合赋值运算，如 C# 中的 (a |= b)。
        OrAssign = 70,
        //
        // 摘要:
        //     对某个数字进行幂运算的复合赋值运算，如 Visual Basic 中的 (a ^= b)。
        PowerAssign = 71,
        //
        // 摘要:
        //     按位右移复合赋值运算，如 (a >>= b)。
        RightShiftAssign = 72,
        //
        // 摘要:
        //     减法复合赋值运算，如 (a -= b)，针对数值操作数，不进行溢出检查。
        SubtractAssign = 73,
        //
        // 摘要:
        //     加法复合赋值运算，如 (a += b)，针对数值操作数，进行溢出检查。
        AddAssignChecked = 74,
        //
        // 摘要:
        //     乘法复合赋值运算，如 (a *= b)，针对数值操作数，进行溢出检查。
        MultiplyAssignChecked = 75,
        //
        // 摘要:
        //     减法复合赋值运算，如 (a -= b)，针对数值操作数，进行溢出检查。
        SubtractAssignChecked = 76,
        //
        // 摘要:
        //     一元前缀递增，如 (++a)。应就地修改 a 对象。
        PreIncrementAssign = 77,
        //
        // 摘要:
        //     一元前缀递减，如 (--a)。应就地修改 a 对象。
        PreDecrementAssign = 78,
        //
        // 摘要:
        //     一元后缀递增，如 (a++)。应就地修改 a 对象。
        PostIncrementAssign = 79,
        //
        // 摘要:
        //     一元后缀递减，如 (a--)。应就地修改 a 对象。
        PostDecrementAssign = 80,
        //
        // 摘要:
        //     确切类型测试。
        TypeEqual = 81,
        //
        // 摘要:
        //     二进制反码运算，如 C# 中的 (~a)。
        OnesComplement = 82,
        //
        // 摘要:
        //     true 条件值。
        IsTrue = 83,
        //
        // 摘要:
        //     false 条件值。
        IsFalse = 84
    }
}
