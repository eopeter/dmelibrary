using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DME.Web.Template.Evaluator
{
    /// <summary>
    /// 表达式的节点(如操作数或运算符)
    /// </summary>
    public class DMEWeb_ExpressionNode
    {
        /// <summary>
        /// 构造节点实例
        /// </summary>
        /// <param name="value">操作数或运算符</param>
        public DMEWeb_ExpressionNode(string value)
        {
            this._Value = value;
            this._Type = ParseNodeType(value);
            this._PRI = GetNodeTypePRI(this.Type);
            this._Numeric = null;
        }

        private string _Value;
        /// <summary>
        /// 返回当前节点的操作数
        /// </summary>
        public string Value
        {
            get
            {
                return _Value;
            }
        }

        private DMEWeb_ExpressionNodeType _Type;
        /// <summary>
        /// 返回当前节点的类型
        /// </summary>
        public DMEWeb_ExpressionNodeType Type
        {
            get
            {
                return _Type;
            }
            internal set
            {
                _Type = value;
            }
        }

        private int _PRI;
        /// <summary>
        /// 返回当前节点的优先级
        /// </summary>
        public int PRI
        {
            get
            {
                return _PRI;
            }
        }

        private object _Numeric;
        /// <summary>
        /// 返回此节点的数值
        /// </summary>
        public object Numeric
        {
            get
            {
                if (_Numeric == null)
                {
                    if (this.Type != DMEWeb_ExpressionNodeType.Numeric) return 0;

                    decimal value = Convert.ToDecimal(this.Value);

                    if (this.UnitaryNode != null)
                    {
                        switch (this.UnitaryNode.Type)
                        {
                            case DMEWeb_ExpressionNodeType.Subtract:
                                value = 0 - value;
                                break;
                        }
                    }

                    _Numeric = value;
                }
                return _Numeric;
            }
            internal set
            {
                _Numeric = value;
                _Value = _Numeric.ToString();
            }
        }

        private DMEWeb_ExpressionNode _UnitaryNode;
        /// <summary>
        /// 设置或返回与当前节点相关联的一元操作符节点
        /// </summary>
        public DMEWeb_ExpressionNode UnitaryNode
        {
            get
            {
                return _UnitaryNode;
            }
            set
            {
                _UnitaryNode = value;
            }
        }

        #region 静态方法与属性
        /// <summary>
        /// 解析节点类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static DMEWeb_ExpressionNodeType ParseNodeType(string value)
        {
            if (string.IsNullOrEmpty(value)) return DMEWeb_ExpressionNodeType.Unknown;

            switch (value)
            {
                case "+":
                    return DMEWeb_ExpressionNodeType.Plus;
                case "-":
                    return DMEWeb_ExpressionNodeType.Subtract;
                case "*":
                    return DMEWeb_ExpressionNodeType.MultiPly;
                case "/":
                    return DMEWeb_ExpressionNodeType.Divide;
                case "%":
                    return DMEWeb_ExpressionNodeType.Mod;
                case "^":
                    return DMEWeb_ExpressionNodeType.Power;
                case "(":
                    return DMEWeb_ExpressionNodeType.LParentheses;
                case ")":
                    return DMEWeb_ExpressionNodeType.RParentheses;
                case "&":
                    return DMEWeb_ExpressionNodeType.BitwiseAnd;
                case "|":
                    return DMEWeb_ExpressionNodeType.BitwiseOr;
                case "&&":
                    return DMEWeb_ExpressionNodeType.And;
                case "||":
                    return DMEWeb_ExpressionNodeType.Or;
                case "!":
                    return DMEWeb_ExpressionNodeType.Not;
                case "==":
                    return DMEWeb_ExpressionNodeType.Equal;
                case "!=":
                case "<>":
                    return DMEWeb_ExpressionNodeType.Unequal;
                case ">":
                    return DMEWeb_ExpressionNodeType.GT;
                case "<":
                    return DMEWeb_ExpressionNodeType.LT;
                case ">=":
                    return DMEWeb_ExpressionNodeType.GTOrEqual;
                case "<=":
                    return DMEWeb_ExpressionNodeType.LTOrEqual;
                case "<<":
                    return DMEWeb_ExpressionNodeType.LShift;
                case ">>":
                    return DMEWeb_ExpressionNodeType.RShift;
                default:
                    //判断是否操作数
                    if (IsNumerics(value))
                    {
                        return DMEWeb_ExpressionNodeType.Numeric;
                    }
                    else
                    {
                        return DMEWeb_ExpressionNodeType.Unknown;
                    }
            }
        }

        /// <summary>
        /// 获取各节点类型的优先级
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        private static int GetNodeTypePRI(DMEWeb_ExpressionNodeType nodeType)
        {
            switch (nodeType)
            {
                case DMEWeb_ExpressionNodeType.LParentheses:
                case DMEWeb_ExpressionNodeType.RParentheses:
                    return 9;
                //逻辑非是一元操作符,所以其优先级较高
                case DMEWeb_ExpressionNodeType.Not:
                    return 8;
                case DMEWeb_ExpressionNodeType.Mod:
                    return 7;
                case DMEWeb_ExpressionNodeType.MultiPly:
                case DMEWeb_ExpressionNodeType.Divide:
                case DMEWeb_ExpressionNodeType.Power:
                    return 6;
                case DMEWeb_ExpressionNodeType.Plus:
                case DMEWeb_ExpressionNodeType.Subtract:
                    return 5;
                case DMEWeb_ExpressionNodeType.LShift:
                case DMEWeb_ExpressionNodeType.RShift:
                    return 4;
                case DMEWeb_ExpressionNodeType.BitwiseAnd:
                case DMEWeb_ExpressionNodeType.BitwiseOr:
                    return 3;
                case DMEWeb_ExpressionNodeType.Equal:
                case DMEWeb_ExpressionNodeType.Unequal:
                case DMEWeb_ExpressionNodeType.GT:
                case DMEWeb_ExpressionNodeType.LT:
                case DMEWeb_ExpressionNodeType.GTOrEqual:
                case DMEWeb_ExpressionNodeType.LTOrEqual:
                    return 2;
                case DMEWeb_ExpressionNodeType.And:
                case DMEWeb_ExpressionNodeType.Or:
                    return 1;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 判断某个操作数是否是数值
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool IsNumerics(string op)
        {
            return Numerics.IsMatch(op);
        }

        /// <summary>
        /// 判断某个字符后是否需要更多的操作符
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool NeedMoreOperator(char c)
        {
            switch (c)
            {
                case '&':
                case '|':
                case '=':
                case '!':
                case '>':
                case '<':
                case '.':   //小数点
                    return true;
            }
            //数字则需要更多
            return char.IsDigit(c);
        }

        /// <summary>
        /// 判断两个字符是否是同一类
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static bool IsCongener(char c1, char c2)
        {
            if (c1 == '(' || c2 == '(') return false;
            if (c1 == ')' || c2 == ')') return false;

            if (char.IsDigit(c1) || c1 == '.')
            {
                //c1为数字,则c2也为数字
                return (char.IsDigit(c2) || c2 == '.');
            }
            else
            {
                //c1为非数字,则c2也为非数字
                return !(char.IsDigit(c2) || c2 == '.');
            }
        }

        /// <summary>
        /// 判断某个字符是否是空白字符
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsWhileSpace(char c)
        {
            return c == ' ' || c == '\t';
        }

        /// <summary>
        /// 判断是否是一元操作符节点
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public static bool IsUnitaryNode(DMEWeb_ExpressionNodeType nodeType)
        {
            return (nodeType == DMEWeb_ExpressionNodeType.Plus || nodeType == DMEWeb_ExpressionNodeType.Subtract);
        }

        /// <summary>
        /// 操作数的正则表达式
        /// </summary>
        private static Regex Numerics = new Regex(@"^[\+\-]?(0|[1-9]\d*|[1-9]\d*\.\d+|0\.\d+)$", RegexOptions.Compiled);
        #endregion
    }
}
