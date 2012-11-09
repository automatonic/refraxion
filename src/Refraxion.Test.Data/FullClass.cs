using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Refraxion.Test.Data
{
    /// <summary>
    /// A test of a class with just about every imaginable member
    /// </summary>
    public class FullClass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FullClass"/> class.
        /// </summary>
        public FullClass()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FullClass"/> class.
        /// </summary>
        /// <param name="intProp">The int prop.</param>
        /// <param name="stringProp">The string prop.</param>
        public FullClass(int intProp, string stringProp)
        {
        }

        /// <summary>
        /// Gets or sets the int prop.
        /// </summary>
        /// <value>The int prop.</value>
        public int IntProp { get; private set; }

        /// <summary>
        /// Gets or sets the string prop.
        /// </summary>
        /// <value>The string prop.</value>
        public string StringProp { get; private set; }

        /// <summary>
        /// Gets or sets the private prop.
        /// </summary>
        /// <value>The private prop.</value>
        private string PrivateProp { get; set; }

        /// <summary>
        /// Gets or sets the protected prop.
        /// </summary>
        /// <value>The protected prop.</value>
        protected string ProtectedProp { get; set; }

        /// <summary>
        /// Simples the method.
        /// </summary>
        public void SimpleMethod()
        {
        }

        /// <summary>
        /// Overloads the method.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public int OverloadMethod(int i)
        {
            return 0;
        }

        /// <summary>
        /// Overloads the method.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public int OverloadMethod(int i, string s)
        {
            return 0;
        }

        /// <summary>
        /// Privates the method.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        private int PrivateMethod(int i)
        {
            return 0;
        }

        /// <summary>
        /// Protecteds the method.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        protected int ProtectedMethod(int i)
        {
            return 0;
        }

        /// <summary>
        /// Occurs when [some event].
        /// </summary>
        public event EventHandler SomeEvent;

        /// <summary>
        /// Occurs when [some other event].
        /// </summary>
        public event EventHandler SomeOtherEvent;

        /// <summary>
        /// Occurs when [some protected event].
        /// </summary>
        protected event EventHandler SomeProtectedEvent;

        /// <summary>
        /// Occurs when [some private event].
        /// </summary>
        private event EventHandler SomePrivateEvent;

        /// <summary>
        /// A nested class
        /// </summary>
        public class NestedClass
        {
            /// <summary>
            /// Gets or sets the int prop.
            /// </summary>
            /// <value>The int prop.</value>
            public int IntProp { get; private set; }

            /// <summary>
            /// Gets or sets the string prop.
            /// </summary>
            /// <value>The string prop.</value>
            public string StringProp { get; private set; }
        }

        /// <summary>
        /// A nested struct
        /// </summary>
        public struct NestedStruct
        {
            /// <summary>
            /// a public int field
            /// </summary>
            public int i;
            /// <summary>
            /// A public string field
            /// </summary>
            public string s;
        }

        /// <summary>
        /// A nested interface
        /// </summary>
        public interface INestedInterface
        {
            /// <summary>
            /// Gets or sets the int prop.
            /// </summary>
            /// <value>The int prop.</value>
            int IntProp { get;}

            /// <summary>
            /// Gets or sets the string prop.
            /// </summary>
            /// <value>The string prop.</value>
            string StringProp { get;}

            /// <summary>
            /// Simples the method.
            /// </summary>
            void SimpleMethod();

            /// <summary>
            /// Overloads the method.
            /// </summary>
            /// <param name="i">The i.</param>
            /// <returns></returns>
            int OverloadMethod(int i);
        }

        /// <summary>
        /// A nested enumeration
        /// </summary>
        public enum NestedEnum
        {
            /// <summary>
            /// a
            /// </summary>
            A,
            /// <summary>
            /// b
            /// </summary>
            B,
            /// <summary>
            /// c
            /// </summary>
            C,
            /// <summary>
            /// d
            /// </summary>
            D
        }
    }
}
