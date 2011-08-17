using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace DME.Base.Collections
{
    /// <summary>
    /// 标识对象，拥有指定标识。（可支持64个独立的标识项）
    /// </summary>
    /// <typeparam name="T">泛型 T （标识）必须是一个枚举。</typeparam>
    /// <remarks>
    /// <para>此类基于二进制操作编写。</para>
    /// <para>要使用此类，只需要定义一个枚举即可，这个枚举表示了各个独立的标识项。但是这个枚举必须满足以下条件：</para>
    /// <ol>
    /// <li>以ulong为基类。</li>
    /// <li>枚举值必须为2的正整数次方或1。</li>
    /// <li>枚举项不超过 64 项。</li>
    /// </ol>
    /// <para>
    /// <strong>* 需特别注意：必须显式地为枚举项提供值，否则若有任何枚举项值为 0，就不符合标识枚举的前提条件了。</strong>
    /// </para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    /// <strong><see cref="DME.Base.Collections.DME_FlagBehavior&lt;T&gt;"/>有广泛的应用场景，能够方便地进行多标识的赋予、剥夺、组合、判断。只要是有多个“是否”判断的情况，均可以使用<see cref="uoLib.Common.FlagBehavior&lt;T&gt;"/>，
    /// 它同时支持高达64个独立的“是否”项，足以满足开发需求。祥见下文示例。</strong>
    /// </para>
    /// <para><i>注：该对象的实例可以在数据库中使用一个bigint字段来存储。使用这个字段的值来初始化<see cref="DME.Base.Collections.DME_FlagBehavior&lt;T&gt;"/>实例，
    /// 经过一系列操作后将该实例的<see cref="DME.Base.Collections.DME_FlagBehavior&lt;T&gt;.FlagU8"/>属性值重新存回数据库即可。</i></para>
    /// 
    /// <h3>用于角色权限判定</h3>
    /// <para>
    /// 需求环境：在系统中我们可能会根据不同的身份来给予管理员不同的操作权限，按照一般的做法比较麻烦，
    /// 使用<see cref="DME.Base.Collections.DME_FlagBehavior&lt;T&gt;"/>可以很轻松地对用户权限进行管理和操作。
    /// </para>
    /// <para><strong>1、定义权限项（标识枚举，泛型 T）：</strong></para>
    /// <code><![CDATA[
    /// [Flags]
    /// public enum MyPowerItem : ulong
    /// {
    ///     查看用户 = 1,
    ///     增改用户 = 2,
    ///     删除用户 = 4,
    ///     
    ///     查看系统日志 = 8,
    ///     管理其他管理员 = 16,
    ///     关闭网站 = 32
    /// }
    /// ]]></code>
    /// 
    /// <para><strong>2、建立权限角色：</strong></para>
    /// <code><![CDATA[
    ///     // A.建立一个无任何权限的角色对象：
    ///     DME_FlagBehavior<MyPowerItem> m = new DME_FlagBehavior<MyPowerItem>();
    ///
    ///     // B.建立初始拥有“查看用户”和“增改用户”权限的角色对象：
    ///     DME_FlagBehavior<MyPowerItem> member = new DME_FlagBehavior<MyPowerItem>(MyPowerItem.查看用户 | MyPowerItem.增改用户);
    /// ]]></code>
    /// 
    /// <para><strong>3、 权限操作</strong></para>
    /// <para>有了上面的权限角色和权限项，我们就可以使用这个类进行权限相关的操作。如：</para>
    /// <para>A. 为此角色对象赋予“查看用户”和“查看系统日志”的权限：</para>
    /// <code><![CDATA[
    ///     // 此操作将在原有的权限基础上赋予新的权限。member的旧权限为“查看用户、增改用户”，现在的新权限为“查看用户、增改用户、查看系统日志”。
    ///     // 即，在赋予权限时，将会把新的权限添加进去，而不会理会旧的权限。
    ///     member.Append(MyPowerItem.查看用户 | MyPowerItem.查看系统日志);
    /// ]]></code>
    /// 
    /// <para>B. 剥夺此角色对象“删除用户”和“增改用户”的标识：</para>
    /// <code><![CDATA[
    ///     // 此操作将从原有的权限中去除指定的权限。member的旧权限为“查看用户、增改用户、查看系统日志”，现在的新权限为“查看用户、查看系统日志”。
    ///     // 即，在剥夺权限时，将会把指定的权限从原有权限中除去，如果原本没有某种权限，将不会理睬。
    ///     member.Remove(MyPowerItem.删除用户 | MyPowerItem.增改用户);
    /// ]]></code>
    /// 
    /// <para>C. 检查角色对象是否具有指定的权限：</para>
    /// <code><![CDATA[
    ///     // 检查是否拥有单独某项权限
    ///     member.Check(MyPowerItem.删除用户);        // false
    ///     member.Check(MyPowerItem.查看用户);        // true
    ///     member.Check(MyPowerItem.查看系统日志);    // true
    ///     
    ///     // 检查是否同时拥有多项权限（A且B的关系）
    ///     if (member.Check(MyPowerItem.查看用户 | MyPowerItem.查看系统日志)) 
    ///     {
    ///         // do something...
    ///     }
    ///     
    ///     // 检查是否拥有多项权限中的一个（A或B的关系）
    ///     if (member.Check(MyPowerItem.查看用户) || member.Check(MyPowerItem.查看系统日志)) 
    ///     {
    ///         // do something...
    ///     }
    /// ]]></code>
    /// <para><span style="color:#FF0000">请注意以上关于多项权限检查的区别。“且”关系可以合并标识枚举一次检查，“或”关系必须分开检查！</span></para>
    /// 
    /// <h3>用于商品/文章属性</h3>
    /// <para>需求环境：一般情况下，商城里的商品、文章可能会有这样的属性：</para>
    /// <ul>
    /// <li>商品：是否热卖、是否特价、是否推荐、是否置顶、是否在首页显示……</li>
    /// <li>文章：是否置顶、是否推荐、是否允许评论、是否为草稿、是否已审核……</li>
    /// </ul>
    /// <para>
    /// 按照一般的经典做法，我们需要在数据库中为每个属性单独设置一个0|1或true|false的字段，或者设置形如“01110101”的字符串按第几位来表示这些属性，
    /// 这样的做法不够科学，数据库存取上也不够高效。但使用<see cref="DME.Base.Collections.DME_FlagBehavior&lt;T&gt;"/>可以让你从这些诸多的属性的判断、解析中解脱出来，
    /// 仅使用一个字段就可以同时表示所有这些“是否”判断，数据库中也仅需要一个bigint字段即可保存这些信息。
    /// </para>
    /// <para>比如你的商品标识枚举是这样的：</para>
    /// <code><![CDATA[
    /// [Flags]
    /// public enum MyProductFlags : ulong
    /// {
    ///     热卖 = 1,
    ///     特价 = 2,
    ///     推荐 = 4,
    ///     置顶 = 8,
    ///     首页显示 = 16
    /// }
    /// ]]></code>
    /// <para>比如你的商品类是这样的：</para>
    /// <code><![CDATA[
    /// public class ProductInfo
    /// {
    ///     public int ID { get; set; }
    ///     public string Name { get; set; }
    ///     public int Price { get; set; }
    ///     public DME_FlagBehavior<MyProductFlags> Flags { get; set; }
    ///
    ///     public static ProductInfo GetItem(int id)
    ///     {
    ///         // 从数据库中读取 ID == id 的商品
    ///         DataRow dr = DataFromDatabase;
    ///         ProductInfo p = new ProductInfo();
    ///         p.ID = int.Parse(dr["ID"].ToString());
    ///         p.Name = dr["Name"].ToString();
    ///         p.Price = int.Parse(dr["Price"].ToString());
    ///         
    ///         // 注意这里的使用方法。
    ///         p.Flags = new DME_FlagBehavior<MyProductFlags>(ulong.Parse(dr["Flags"].ToString()));
    ///
    ///         return p;
    ///     }
    ///
    ///     public void Save()
    ///     {
    ///         // 更新到数据库
    ///         SqlCommand cmd = new SqlCommand();
    ///         cmd.CommandText = "UPDATE [TABLE] SET Name = @name,Price=@pirce,Flags=@flags WHERE ID=@id";
    ///         cmd.Parameters.Add(new SqlParameter("@id", this.ID));
    ///         cmd.Parameters.Add(new SqlParameter("@name", this.Name));
    ///         cmd.Parameters.Add(new SqlParameter("@pirce", this.Price));
    ///         
    ///         // 注意这里的使用方法。
    ///         cmd.Parameters.Add(new SqlParameter("@flags", this.Flags.FlagU8));
    ///
    ///         // 执行 SqlCommand 略...
    ///     }
    /// }
    /// ]]></code>
    /// <para>如此，你可以这样判断一个商品的属性</para>
    /// <code><![CDATA[
    ///     // 获取一个商品实例，请注意如何初始化这个实例的标识属性
    ///     ProductInfo product = ProductInfo.GetItem(1);
    ///     
    ///     // 检查商品是否同时是热卖和特价的商品
    ///     product.Flags.Check(MyProductFlags.热卖 | MyProductFlags.推荐);
    ///     
    ///     // 设置商品为特价商品
    ///     product.Flags.Append(MyProductFlags.特价);
    ///     
    ///     // 设置商品为置顶和首页显示
    ///     product.Flags.Append(MyProductFlags.置顶 | MyProductFlags.首页显示);
    ///     
    ///     // 将商品存入数据库，请注意如何将标识属性存回数据库。
    ///     prodcut.Save();
    /// ]]></code>
    /// 
    /// <h3>Check操作的前置/后置事件：</h3>
    /// <para>此功能增强了Check操作的效力。当Check操作需要前提条件、后置条件时，使用
    /// 事件 <see cref="DME.Base.Collections.DME_FlagBehavior&lt;T&gt;.BeforeCheck"/>、<see cref="DME.Base.Collections.DME_FlagBehavior&lt;T&gt;.AfterCheck"/> 将能使您的程序逻辑更清晰，
    /// 代码更简洁，重用性能更高。</para>
    /// <para>具体来讲，仍然拿权限判定来说。如果权限系统有更复杂的管理组，则可能有这样的情况：该管理员组具有A权限，但组本身已经被禁用。
    /// 如果此时我们要判断该组内管理员是否具有A权限，则需要先判断此管理员组是否已经被禁用，在未被禁用情况下才去判断A权限。这样只稍稍复杂
    /// 一点的逻辑，将导致我们多次在代码里写“管理员组是否已经被禁用”的判断。特别是这样的权限判断在多个页面、多处调用时，这将导致代码
    /// 冗余、混乱。</para>
    /// <para>此时，Check操作的前置/后置事件就可以派上大用场了，它能统一管理Check操作的前置判断和后置判断。您无需在多处代码写“管理员组是否已经被禁用”的判断，
    /// 一处就搞定了。这对于代码执行、重用、维护，都有很大好处。</para>
    /// <para>具体示例见下：</para>
    /// <code><![CDATA[
    /// using System;
    /// using DME.Base.Collections;
    /// 
    /// namespace FlagBehavior_BeforeCheck
    /// {
    ///     class Program
    ///     {
    ///         private static ManagerGroup myGroup;
    /// 
    ///         static void Main(string[] args)
    ///         {
    ///             // 假定有如下管理员组，同时具有“查看”和“删除”的权限，但管理员组本身被禁用
    ///             myGroup = new ManagerGroup()
    ///             {
    ///                 GroupName = "测试用户组by uonun",
    ///                 GroupPower = new DME_FlagBehavior<Power>(Power.查看 | Power.删除),
    ///
    ///                 // 注意此处已设置用户组被禁用
    ///                 Enabled = false
    ///             };
    ///
    ///             /*
    ///              * 一般的做法，要判断此管理员组是否有“查看”的权限，需要先检查该管理员组本身是否被禁用，
    ///              * 之后再检查管理员组的对应权限。如果权限检查操作在程序的多个地方执行，则每个地方都需要
    ///              * 对管理员组本身是否被禁用进行检查。
    ///              *
    ///              *   bool isOk;
    ///              *   if (myGroup.Enabled == true)
    ///              *   {
    ///              *       isOk = myGroup.GroupPower.Check(ManagerPower.查看用户);
    ///              *   }
    ///              *   else {
    ///              *       isOk = false;
    ///              *   }
    ///              * 
    ///              * 
    ///              * 但您使用Check操作的前置事件，则可以优化程序，使程序逻辑更清晰，代码更简洁。
    ///              */
    ///             myGroup.GroupPower.BeforeCheck += new DME_FlagBehavior<Power>.BeforeCheckEventHandler(GroupPower_BeforeCheck);
    ///
    ///             bool isOk;
    ///             isOk = myGroup.GroupPower.Check(Power.查看);
    ///
    ///             Console.WriteLine(isOk);
    ///             Console.ReadKey();
    ///         }
    ///
    ///         static bool GroupPower_BeforeCheck(DME_FlagBehavior<Power> sender, DME_FlagBehavior<Power>.BeforeCheckEventArgs e)
    ///         {
    ///             // 当管理员组启用时，返回 true，Check操作将会继续对指定的权限标识进行检查。
    ///             // 否则Check操作直接返回false。
    ///             return myGroup.Enabled == true;
    ///         }
    ///
    ///     }
    ///
    ///     class ManagerGroup
    ///     {
    ///         public string GroupName;
    ///         public DME_FlagBehavior<Power> GroupPower;
    ///         public bool Enabled;
    ///     }
    ///
    ///     [Flag]
    ///     enum Power : ulong
    ///     {
    ///         查看 = 0x1,
    ///         删除 = 0x2
    ///     }
    /// }
    /// ]]></code>
    /// 
    /// <para>
    /// <strong>总之，<see cref="DME.Base.Collections.DME_FlagBehavior&lt;T&gt;"/>有广泛的应用场景，只要是有多个“是否”判断的情况，
    /// 均可以使用<see cref="DME.Base.Collections.DME_FlagBehavior&lt;T&gt;"/>，它同时支持高达64个独立的“是否”项，足以满足开发需求。</strong>
    /// </para>
    /// <para>至于在数据库中针对标识字段的查询，如果使用SQL语句查询，可以使用<see cref="DME.Base.Collections.DME_FlagBehavior&lt;T&gt;.GetSqlConditions(string, T)"/>来构造。如果使用C#3.5及其以上的lambda表达式，直接使用uoLib.Common.FlagBehavior&lt;T&gt;.Check(T)即可。</para>
    /// <para>另外，为方便调试输出，<see cref="DME.Base.Collections.DME_FlagBehavior&lt;T&gt;"/>重写了ToString方法用于输出标识细节。例如输出上文最后状态的 member.ToString()，则会有：</para>
    /// <code>
    /// 标识
    /// -------------------------------------
    /// 标识值（ulong）：9
    /// 二进制字符串：1001
    /// 标识详情：（查看用户, 查看系统日志）
    ///         查看用户：True
    ///         增改用户：False
    ///         删除用户：False
    ///         查看系统日志：True
    ///         管理其他管理员：False
    ///         关闭网站：False
    /// </code>
    /// </example>
    /// <exception cref="ArgumentException">泛型 T （标识）必须是一个枚举！</exception>
    /// <exception cref="ArgumentException">泛型 T （标识）所指定的枚举其基础类型必须是 ulong ！</exception>
    /// <exception cref="ArgumentException">泛型 T （标识）的值必须是2的正整数次方或1，且不能重复！</exception>
    /// <exception cref="ArgumentOutOfRangeException">参数必须大于 -1 ！</exception>
    /// <exception cref="ArgumentOutOfRangeException">参数必须是由0、1构成的二进制字符串！</exception>
    public class DME_FlagBehavior<T> where T : IComparable, IConvertible, IFormattable
    {
        #region CONST
        private const string TTypeErr = "泛型 T （标识）必须是一个枚举！";
        private const string TBaseErr = "泛型 T （标识）所指定的枚举其基础类型必须是 ulong ！";
        private const string TValueErr = "泛型 T （标识）的值必须是2的正整数次方或1，且不能重复！";
        private const string MinFlagErr = "参数必须大于 -1 ！";
        private const string FlagStringErr = "参数必须是由0、1构成的二进制字符串！";

        /// <summary>
        /// 一个标识，此值的二进制由64个1构成，也就是说它拥有泛型 <typeparam name="T" /> 中的所有标识。比如应用在权限模块中它就表示“拥有所有权限”；
        /// </summary>
        public const ulong FullFlagU8 = 0xFFFFFFFFFFFFFFFF;
        #endregion

        #region 属性
        /// <summary>
        /// 获取当前对象的标识结果（注意：不一定是泛型 T 的某一项，也可能是 T 的组合结果）。
        /// </summary>
        public T Flag
        {
            get { return _flag; }
            private set
            {
                #region 检查枚举值的合法性
                Array values = Enum.GetValues(value.GetType());
                ulong tmpV = 0;
                //检查泛型的每一个值的合法性
                for (int n = 0;n < values.Length;n++)
                {
                    tmpV = TtoU8((T)values.GetValue(n));
                    //检查是否为2的正整数次方或1
                    if (!IsPowerOfTwo(tmpV) && tmpV != 1) { throw new ArgumentException(TValueErr); }

                    //检查当前值是否重复
                    for (int k = 0;k < n;k++)
                    {
                        if (tmpV == TtoU8((T)values.GetValue(k)))
                        {
                            throw new ArgumentException(TValueErr);
                        }
                    }
                }
                #endregion
                _flag = value;
            }
        }
        private T _flag;

        /// <summary>
        /// 获取当前对象的标识结果（二进制字符串形式）
        /// </summary>
        public string FlagString
        {
            get
            {
                ulong v = TtoU8(this.Flag);
                return UlongToStr(v);
            }
        }

        /// <summary>
        /// 获取当前对象的标识结果（ulong 形式）
        /// </summary>
        public ulong FlagU8 { get { return TtoU8(this.Flag); } }

        /// <summary>
        /// 一个标识，此值的二进制由64个1构成，也就是说它拥有泛型 <typeparam name="T" /> 中的所有标识。比如应用在权限模块中它就表示“拥有所有权限”；
        /// </summary>
        public static T FullFlag
        {
            get { return (T)Enum.Parse(typeof(T), FullFlagU8.ToString()); ; }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造一个标识控制对象，无任何标识。
        /// </summary>
        public DME_FlagBehavior()
        {
            if (Enum.GetUnderlyingType(typeof(T)) != typeof(UInt64)) { throw new ArgumentException(TBaseErr); }

            T tmp = (T)Enum.Parse(typeof(T), "0");
            this.Flag = tmp;
        }
        /// <summary>
        /// 构造一个属性标识对象，标识由标识枚举指定。
        /// </summary>
        /// <param name="ownedFlag">该角色拥有的标识。如：“<typeparamref name="T"/>.查看用户 | <typeparamref name="T"/>.增改用户”</param>
        public DME_FlagBehavior(T ownedFlag)
        {
            if (!(ownedFlag.GetType().IsEnum)) throw new ArgumentException(TTypeErr);
            if (Enum.GetUnderlyingType(typeof(T)) != typeof(UInt64)) { throw new ArgumentException(TBaseErr); }

            Flag = ownedFlag;
        }
        /// <summary>
        /// 构造一个属性标识对象，标识由标识标识指定（标识将被转换为二进制形式）。
        /// </summary>
        /// <param name="ownedFlag">该角色拥有的标识（标识，将被转换为二进制形式）</param>
        public DME_FlagBehavior(ulong ownedFlag)
        {
            if (ownedFlag < 0) { throw new ArgumentOutOfRangeException(MinFlagErr); }
            if (Enum.GetUnderlyingType(typeof(T)) != typeof(UInt64)) { throw new ArgumentException(TBaseErr); }

            T tmp = (T)Enum.Parse(typeof(T), ownedFlag.ToString());
            this.Flag = tmp;
        }
        /// <summary>
        /// 构造一个属性标识对象，标识由二进制字符串指定。
        /// </summary>
        /// <param name="ownedFlag">该角色拥有的标识（二进制如：“10000100000”）</param>
        public DME_FlagBehavior(string ownedFlag)
        {
            ulong p = 0UL;

            if (!string.IsNullOrEmpty(ownedFlag) && ownedFlag.Trim().Length > 0)
            {
                if (ownedFlag.Trim().Replace("0", "").Replace("1", "").Length != 0)
                    throw new ArgumentOutOfRangeException(FlagStringErr);

                p = Convert.ToUInt64(ownedFlag, 2);
            }
            T tmp = (T)Enum.Parse(typeof(T), p.ToString());
            this.Flag = tmp;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 检查当前对象是否满足指定标识要求。
        /// </summary>
        /// <param name="holdedFlags">标识集</param>
        /// <param name="targetFlag">目标标识</param>
        /// <remarks>
        /// 此静态方法相当于<see cref="uoLib.Common.FlagBehavior&lt;T&gt;.Check(T)"/>的简化版，
        /// <strong>它仅比较两个标识，不会触发<see cref="uoLib.Common.FlagBehavior&lt;T&gt;.AfterCheck"/>、<see cref="uoLib.Common.FlagBehavior&lt;T&gt;.BeforeCheck"/>。</strong>
        /// 只需要比较标识而不对标识进行其他操作时，建议使用此方法。
        /// </remarks>
        /// <returns>满足返回true，否则返回false。<br />当 <paramref name="targetFlage"/> 的值为 0 时，返回ture。</returns>
        public static bool Check(T holdedFlags, T targetFlag)
        {
            if (targetFlag.ToString() == "0") { return true; }

            ulong n = TtoU8(targetFlag);
            ulong myFlag = TtoU8(holdedFlags);

            //用户拥有的标识：  10110
            //操作需要的标识：  10010
            //& 操作    -------------
            //                  10010
            //        = 操作需要的标识
            return ((myFlag & n) == n);
        }

        /// <summary>
        /// 检查当前对象是否满足指定标识要求。<br />
        /// 注意：此方法将会触发 <see cref="uoLib.Common.FlagBehavior&lt;T&gt;.BeforeCheck"/>、<see cref="uoLib.Common.FlagBehavior&lt;T&gt;.AfterCheck"/> 
        /// 两个事件，并根据两个事件的返回值影响此方法的返回值。具体请查看SDK文档。
        /// </summary>
        /// <param name="flag">需要满足的标识</param>
        /// <remarks>
        /// <para>
        /// 此方法是本模块的核心方法，它检查当前对象是否满足指定标识的要求。其返回值受事件：
        /// <see cref="uoLib.Common.FlagBehavior&lt;T&gt;.BeforeCheck"/>、<see cref="uoLib.Common.FlagBehavior&lt;T&gt;.AfterCheck"/> 影响。
        /// 具体表现为：
        /// </para>
        /// <ul>
        /// <li>当事件 <see cref="uoLib.Common.FlagBehavior&lt;T&gt;.BeforeCheck"/>、<see cref="uoLib.Common.FlagBehavior&lt;T&gt;.AfterCheck"/> 均
        /// 为null时，直接对传入标识进行检查。满足当前对象标识则返回true，否则返回false。</li>
        /// <li>当事件 <see cref="uoLib.Common.FlagBehavior&lt;T&gt;.BeforeCheck"/> 不为null，则首先触发该事件。若该事件返回true，则继续进行后续针对传入标识的检查操作，
        /// 否则直接返回false，并且不触发 <see cref="uoLib.Common.FlagBehavior&lt;T&gt;.AfterCheck"/></li>
        /// <li>只有当 <see cref="uoLib.Common.FlagBehavior&lt;T&gt;.BeforeCheck"/> 返回true，并且 <see cref="uoLib.Common.FlagBehavior&lt;T&gt;.AfterCheck"/> 不
        /// 为null时，<see cref="uoLib.Common.FlagBehavior&lt;T&gt;.AfterCheck"/> 才会被触发。
        /// 并且最终 <see cref="uoLib.Common.FlagBehavior&lt;T&gt;.Check(T)"/> 将返回 <see cref="uoLib.Common.FlagBehavior&lt;T&gt;.AfterCheck"/> 的值。</li>
        /// </ul>
        /// </remarks>
        /// <returns>满足返回true，否则返回false</returns>
        public bool Check(T flag)
        {
            bool result = true;
            if (this.BeforeCheck != null)
                result = this.BeforeCheck(this, new DME_FlagBehavior<T>.BeforeCheckEventArgs() { Flag = flag });

            if (result == true)
            {
                result = DME_FlagBehavior<T>.Check(this.Flag, flag);

                if (this.AfterCheck != null)
                {
                    return this.AfterCheck(this, new DME_FlagBehavior<T>.AfterCheckEventArgs() { CheckedFlag = flag, CheckResult = result });
                }

                return result;
            }
            else { return false; }
        }
        /// <summary>
        /// 给现有角色对象赋予一个标识
        /// </summary>
        /// <param name="flag">要赋予的标识</param>
        public void Append(T flag)
        {
            ulong n = TtoU8(_flag);
            ulong myFlag = TtoU8(flag);
            _flag = (T)Enum.Parse(typeof(T), (n | myFlag).ToString());
        }
        /// <summary>
        /// 从现有角色对象剥夺一个标识
        /// </summary>
        /// <param name="flag"></param>
        public void Remove(T flag)
        {
            ulong n = TtoU8(_flag);
            ulong myFlag = TtoU8(flag);
            _flag = (T)Enum.Parse(typeof(T), (n ^ (n & myFlag)).ToString());
        }
        /// <summary>
        /// 切换现有角色对象的某个标识状态
        /// </summary>
        /// <param name="flag"></param>
        public void Toggle(T flag)
        {
            if (DME_FlagBehavior<T>.Check(this.Flag, flag))
            {
                this.Remove(flag);
            }
            else
            {
                this.Append(flag);
            }
        }

        /// <summary>
        /// 获取SQL语句的查询条件部分。（用于筛选满足此标识条件的记录，具体说明详见SDK文档）
        /// </summary>
        /// <param name="column">标识数据在数据库中的列名称</param>
        /// <param name="flags">要检查的标识值</param>
        /// <remarks>
        /// <para>使用此方法可以获取用于构造SQL语句的条件部分，典型应用比如：</para>
        /// <ol>
        /// <li>假定数据库商品表名为：Products</li>
        /// <li>表Products中有名为ProductFlags的列用于表示这个商品的标识属性（热卖、推荐、特价……）</li>
        /// </ol>
        /// <para>需求1：查询所有特价商品</para>
        /// <code><![CDATA[
        ///     string condition = FlagBehavior<MyProductFlags>.GetSqlConditions("ProductFlags",MyProductFlags.特价);
        ///     // condition == " [ProductFlags] & 2 = 2 "
        ///     string sql = string.Format("SELECT TOP 1 * FROM [Products] WHERE {0}",condition);
        /// ]]></code>
        /// <para>需求2：查询所有特价<strong>且</strong>热卖的商品</para>
        /// <code><![CDATA[
        ///     string condition = FlagBehavior<MyProductFlags>.GetSqlConditions("ProductFlags",MyProductFlags.特价 | MyProductFlags.热卖);
        ///     // condition == " [ProductFlags] & 3 = 3 "
        ///     string sql = string.Format("SELECT TOP 1 * FROM [Products] WHERE {0}",condition);
        /// ]]></code>
        /// <para>需求3：查询所有特价<strong>或</strong>热卖的商品</para>
        /// <code><![CDATA[
        ///     string condition1 = FlagBehavior<MyProductFlags>.GetSqlConditions("ProductFlags",MyProductFlags.特价);
        ///     string condition2 = FlagBehavior<MyProductFlags>.GetSqlConditions("ProductFlags",MyProductFlags.热卖);
        ///
        ///     // condition1 == " [ProductFlags] & 2 = 2 "
        ///     // condition2 == " [ProductFlags] & 1 = 1 "
        ///     string sql = string.Format("SELECT TOP 1 * FROM [Products] WHERE {0} OR {1}",condition1,condition2);
        /// ]]></code>
        /// <para><span style="color:#FF0000">请注意以上需求2与需求3的区别。“且”关系可以合并标识枚举一次性构造条件字符串，“或”关系必须分开构造！</span></para>
        /// </remarks>
        /// <returns></returns>
        public static string GetSqlConditions(string column, T flags)
        {
            if (DME.Base.Helper.DME_Validation.IsNull(column)) throw new ArgumentException("column");
            if (flags.Equals(null)) throw new ArgumentNullException("flags");

            return string.Format(" [{0}] & {1} = {1} ", column, TtoU8(flags));
        }
        #endregion

        #region 事件、委托
        /// <summary>
        /// 在 <see cref="uoLib.Common.FlagBehavior&lt;T&gt;.Check(T)"/> 操作执行前发生。一般用于“检查标识”动作有前置操作的情况。
        /// </summary>
        public event BeforeCheckEventHandler BeforeCheck;
        /// <summary>
        /// 在 <see cref="uoLib.Common.FlagBehavior&lt;T&gt;.Check(T)"/> 操作执行后发生。一般用于“检查标识”动作有后置操作的情况。
        /// </summary>
        public event AfterCheckEventHandler AfterCheck;
        /// <summary>
        /// 表示将对 <see cref="uoLib.Common.FlagBehavior&lt;T&gt;"/> 的以下事件进行处理的方法：<see cref="BeforeCheck"/>。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public delegate bool BeforeCheckEventHandler(DME_FlagBehavior<T> sender, BeforeCheckEventArgs e);
        /// <summary>
        /// 表示将对 <see cref="uoLib.Common.FlagBehavior&lt;T&gt;"/> 的以下事件进行处理的方法：<see cref="AfterCheck"/>。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public delegate bool AfterCheckEventHandler(DME_FlagBehavior<T> sender, AfterCheckEventArgs e);
        /// <summary>
        /// 为 <see cref="BeforeCheck"/> 事件提供数据。
        /// </summary>
        public class BeforeCheckEventArgs : EventArgs
        {
            /// <summary>
            /// 将要检查的标识
            /// </summary> 
            public T Flag;
        }
        /// <summary>
        /// 为 <see cref="AfterCheck"/> 事件提供数据。
        /// </summary>
        public class AfterCheckEventArgs : EventArgs
        {
            /// <summary>
            /// 刚刚检查的标识
            /// </summary>
            public T CheckedFlag;
            /// <summary>
            /// 刚刚检查的结果
            /// </summary>
            public bool CheckResult;
        }
        #endregion

        #region 重载
        /// <summary>
        /// 查看当前对象的标识详情
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("标识");
            sb.AppendLine("-------------------------------------");
            sb.AppendLine("标识值（ulong）：" + this.FlagU8);
            sb.AppendLine("二进制字符串：" + this.FlagString);

            sb.AppendLine("标识详情：（" + this.Flag + "）");
            Array powers = Enum.GetValues(typeof(T));
            IEnumerator ie = powers.GetEnumerator();
            T tmpP;
            while (ie.MoveNext())
            {
                tmpP = (T)Enum.Parse(typeof(T), ie.Current.ToString());
                if (Check(tmpP))
                {
                    sb.AppendLine("\t" + tmpP.ToString() + "：True");
                }
                else
                {
                    sb.AppendLine("\t" + tmpP.ToString() + "：False");
                }
            }
            return sb.ToString();
        }
        #endregion

        #region 私有
        /// <summary>
        /// 检查是否为2的正整数次方
        /// </summary>
        /// <param name="value">待检验的正整数</param>
        /// <returns></returns>
        private static bool IsPowerOfTwo(ulong value)
        {
            if (value > 1)
            {
                return ((value - 1) & value) == 0;
            }
            return false;
        }

        /*
        //检查是否为2的正整数次方
        private static bool IsPowerOfTwo_Obsolete(ulong value)
        {
            if (value > 1)
            {
                * 
                 * 针对每一个 ulong 的正整数，如果是2的正整数次方，则转换为二进制表示时一定有且仅有一个“1”存在。
                 * 基于以上原理，则可以按位来比较。
                 * 从低位到高位查询，遇到“1”后停止比较，所得到的数应当与原数相等，
                 * 否则该数的二进制形式不止一个“1”位，即该数一定不是2的正整数次方。
                 *
                ulong mask = 1ul;
                for (int count = 0;count < 64;count++)
                {
                    //检查当前标识位是否为“0”
                    if ((mask & value) == 0)
                    {
                        //当前位为“0”，则左移标识位，再次检查
                        mask <<= 1;
                        continue;
                    }

                    * 
                     * 程序执行到此，表明当前标识位为“1”，停止检查。
                     * 而此时的 mask 值正好应当与 value 相等，否则 value 就不是2的正整数次方。
                     *
                    return mask == value;
                }
            }
            return false;
        }
        */

        /// <summary>
        /// 将64位无符号整数（ulong）转换为二进制字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string UlongToStr(ulong value)
        {
            int size = 64;
            ulong mask = 1ul << size - 1;
            StringBuilder sb = new StringBuilder();
            string result = string.Empty;
            for (int count = 0;count < size;count++)
            {
                result = ((mask & value) > 0) ? "1" : "0";
                sb.Append(result);

                mask >>= 1;
            }
            return sb.ToString().TrimStart('0');
        }
        /// <summary>
        /// 将标识对象转化为长整形形式
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static ulong TtoU8(T obj)
        {
            return (ulong)Enum.Parse(typeof(T), obj.ToString());
        }
        #endregion
    }
}
