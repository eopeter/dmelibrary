using System;
using System.Collections.Generic;
using System.Text;

namespace DME.Web.Template.Evaluator
{
    /// <summary>
    /// 表达式错误
    /// </summary>
    public class DMEWeb_ExpressionException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public DMEWeb_ExpressionException()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public DMEWeb_ExpressionException(string message)
            : base(message)
        { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public DMEWeb_ExpressionException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
