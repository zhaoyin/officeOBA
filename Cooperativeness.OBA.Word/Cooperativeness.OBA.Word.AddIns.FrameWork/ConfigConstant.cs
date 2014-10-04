
namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义常量对象
    /// </summary>
    public class ConfigConstant
    {

        // 框架默认启动级别
        public static readonly int DEFAULT_INITIAL_STARTLEVEL = 6; 

        #region 包设置
        /// <summary>
        /// 定义插件默认存放的文件夹名
        /// </summary>
        public const string PLUGINS_DIR_NAME = "plugins";
        public const string BUNDLE_MF = "MANIFEST.MF";
        public const string BUNDLE_MF_HOME = "META-INF";
        #endregion

        public const string PREFIX_NS = "Cooperativeness.OBA.Word.AddIns.FrameWork.MetaData.X";

        /// <summary>
        /// 系统插件资源清单名称
        /// </summary>
        public const string SYSTEM_MANIFEST = "Cooperativeness.OBA.Word.AddIns.FrameWork.META_INF.MANIFEST.MF";


        /// <summary>
        /// 系统插件定位标识
        /// </summary>
        public const string SYSTEM_BUNDLE_LOCATION = "System Bundle";

        /// <summary>
        /// 系统插件标识名
        /// </summary>
        public const string SYSTEM_BUNDLE_SYMBOLICNAME = "system.bundle";

        /// <summary>
        /// 插件清单头信息中标识的插件分类
        /// </summary>
        public const string BUNDLE_CATEGORY = "Bundle-Category";

        /// <summary>
        /// 插件清单头信息中标识的插件的版权
        /// </summary>
        public const string BUNDLE_COPYRIGHT = "Bundle-Copyright";

        /// <summary>
        /// 插件清单头信息中标识的插件描述
        /// </summary>
        public const string BUNDLE_DESCRIPTION = "Bundle-Description";

        /// <summary>
        /// 插件清单头信息中标识的插件的名称
        /// </summary>
        public const string BUNDLE_NAME = "Bundle-Name";

        /// <summary>
        /// 插件清单头信息中标识的插件提供者
        /// </summary>
        public const string BUNDLE_VENDOR = "Bundle-Vendor";

        /// <summary>
        /// 插件清单头信息中标识的插件的版本信息
        /// </summary>
        public const string BUNDLE_VERSION = "Bundle-Version";

        /// <summary>
        ///  插件清单头信息中标识的插件激活器
        /// </summary>
        public const string BUNDLE_ACTIVATOR = "Bundle-Activator";

        /// <summary>
        /// 插件清单头信息中标识的插件标识名
        /// </summary>
        public const string BUNDLE_SYMBOLICNAME = "Bundle-SymbolicName";

        /// <summary>
        /// 插件清单头信息中标识的插件的单例
        /// </summary>
        public const string SINGLETON_DIRECTIVE = "singleton";

        /// <summary>
        /// 插件包管理服务接口名
        /// </summary>
        public const string BUNDLE_PACKAGEADMIN_NAME = "Cooperativeness.OBA.Word.AddIns.FrameWork.Package.IPackageAdmin";

        /// <summary>
        /// 插件启动等级服务接口名
        /// </summary>
        public const string BUNDLE_STARTLEVEL_NAME = "Cooperativeness.OBA.Word.AddIns.FrameWork.IStartLevel";
        

        /**
         * Manifest header directive identifying if and when a fragment may attach
         * to a host bundle. The default value is
         * {@link #FRAGMENT_ATTACHMENT_ALWAYS always}.
         * 
         * <p>
         * The directive value is encoded in the Bundle-SymbolicName manifest header
         * like:
         * 
         * <pre>
         *     Bundle-SymbolicName: com.acme.module.test; fragment-attachment:=&quot;never&quot;
         * </pre>
         * 
         * @see #BUNDLE_SYMBOLICNAME
         * @see #FRAGMENT_ATTACHMENT_ALWAYS
         * @see #FRAGMENT_ATTACHMENT_RESOLVETIME
         * @see #FRAGMENT_ATTACHMENT_NEVER
         * @since 1.3
         */
        public const string FRAGMENT_ATTACHMENT_DIRECTIVE = "fragment-attachment";

        /**
         * Manifest header directive value identifying a fragment attachment type of
         * always. A fragment attachment type of always indicates that fragments are
         * allowed to attach to the host bundle at any time (while the host is
         * resolved or during the process of resolving the host bundle).
         * 
         * <p>
         * The directive value is encoded in the Bundle-SymbolicName manifest header
         * like:
         * 
         * <pre>
         *     Bundle-SymbolicName: com.acme.module.test; fragment-attachment:=&quot;always&quot;
         * </pre>
         * 
         * @see #FRAGMENT_ATTACHMENT_DIRECTIVE
         * @since 1.3
         */
        public const string FRAGMENT_ATTACHMENT_ALWAYS = "always";

        /**
         * Manifest header directive value identifying a fragment attachment type of
         * Resolve-time. A fragment attachment type of Resolve-time indicates that
         * fragments are allowed to attach to the host bundle only during the
         * process of resolving the host bundle.
         * 
         * <p>
         * The directive value is encoded in the Bundle-SymbolicName manifest header
         * like:
         * 
         * <pre>
         *     Bundle-SymbolicName: com.acme.module.test; fragment-attachment:=&quot;Resolve-time&quot;
         * </pre>
         * 
         * @see #FRAGMENT_ATTACHMENT_DIRECTIVE
         * @since 1.3
         */
        public const string FRAGMENT_ATTACHMENT_RESOLVETIME = "resolve-time";

        /**
         * Manifest header directive value identifying a fragment attachment type of
         * never. A fragment attachment type of never indicates that no fragments
         * are allowed to attach to the host bundle at any time.
         * 
         * <p>
         * The directive value is encoded in the Bundle-SymbolicName manifest header
         * like:
         * 
         * <pre>
         *     Bundle-SymbolicName: com.acme.module.test; fragment-attachment:=&quot;never&quot;
         * </pre>
         * 
         * @see #FRAGMENT_ATTACHMENT_DIRECTIVE
         * @since 1.3
         */
        public const string FRAGMENT_ATTACHMENT_NEVER = "never";

        /**
         * Manifest header identifying the base name of the bundle's localization
         * entries.
         * 
         * <p>
         * The attribute value may be retrieved from the <code>Dictionary</code>
         * object returned by the <code>Bundle.Headers</code> method.
         * 
         * @see #BUNDLE_LOCALIZATION_DEFAULT_BASENAME
         * @since 1.3
         */
        public const string BUNDLE_LOCALIZATION = "Bundle-Localization";

        /**
         * Default value for the <code>Bundle-Localization</code> manifest header.
         * 
         * @see #BUNDLE_LOCALIZATION
         * @since 1.3
         */
        public const string BUNDLE_LOCALIZATION_DEFAULT_BASENAME = "OSGI-INF/l10n/bundle";

        /**
         * Manifest header identifying the symbolic names of other bundles required
         * by the bundle.
         * 
         * <p>
         * The attribute value may be retrieved from the <code>Dictionary</code>
         * object returned by the <code>Bundle.Headers</code> method.
         * 
         * @since 1.3
         */
        public const string REQUIRE_BUNDLE = "Require-Bundle";

        /**
         * Manifest header attribute identifying a range of versions for a bundle
         * specified in the <code>Require-Bundle</code> or
         * <code>Fragment-Host</code> manifest headers. The default value is
         * <code>0.0.0</code>.
         * 
         * <p>
         * The attribute value is encoded in the Require-Bundle manifest header
         * like:
         * 
         * <pre>
         *     Require-Bundle: com.acme.module.test; bundle-version=&quot;1.1&quot;
         *     Require-Bundle: com.acme.module.test; bundle-version=&quot;[1.0,2.0)&quot;
         * </pre>
         * 
         * <p>
         * The bundle-version attribute value uses a mathematical interval notation
         * to specify a range of bundle versions. A bundle-version attribute value
         * specified as a single version means a version range that includes any
         * bundle version greater than or equal to the specified version.
         * 
         * @see #REQUIRE_BUNDLE
         * @since 1.3
         */
        public const string BUNDLE_VERSION_ATTRIBUTE = "bundle-version";

        /**
         * Manifest header identifying the symbolic name of another bundle for which
         * that the bundle is a fragment.
         * 
         * <p>
         * The attribute value may be retrieved from the <code>Dictionary</code>
         * object returned by the <code>Bundle.Headers</code> method.
         * 
         * @since 1.3
         */
        public const string FRAGMENT_HOST = "Fragment-Host";

        /**
         * Manifest header attribute is used for selection by filtering based upon
         * system property.
         * 
         * <p>
         * The attribute value is encoded in manifest headers like:
         * 
         * <pre>
         *     Bundle-NativeCode: libgtk.so; selection-filter=&quot;(ws=gtk)&quot;; ...
         * </pre>
         * 
         * @see #BUNDLE_NATIVECODE
         * @since 1.3
         */
        public const string SELECTION_FILTER_ATTRIBUTE = "selection-filter";

        /**
         * Manifest header identifying the bundle manifest version. A bundle
         * manifest may express the version of the syntax in which it is written by
         * specifying a bundle manifest version. Bundles exploiting OSGi Release 4,
         * or later, syntax must specify a bundle manifest version.
         * <p>
         * The bundle manifest version defined by OSGi Release 4 or, more
         * specifically, by version 1.3 of the OSGi Core Specification is "2".
         * 
         * <p>
         * The attribute value may be retrieved from the <code>Dictionary</code>
         * object returned by the <code>Bundle.Headers</code> method.
         * 
         * @since 1.3
         */
        public const string BUNDLE_MANIFESTVERSION = "Bundle-ManifestVersion";

        /**
         * Manifest header attribute identifying the version of a package specified
         * in the Export-Package or Import-Package manifest header.
         * 
         * <p>
         * The attribute value is encoded in the Export-Package or Import-Package
         * manifest header like:
         * 
         * <pre>
         *     Import-Package: org.osgi.framework; version=&quot;1.1&quot;
         * </pre>
         * 
         * @see #EXPORT_PACKAGE
         * @see #IMPORT_PACKAGE
         * @since 1.3
         */
        public const string VERSION_ATTRIBUTE = "version";

        /**
         * Manifest header attribute identifying the symbolic name of a bundle that
         * exports a package specified in the Import-Package manifest header.
         * 
         * <p>
         * The attribute value is encoded in the Import-Package manifest header
         * like:
         * 
         * <pre>
         *     Import-Package: org.osgi.framework; bundle-symbolic-name=&quot;com.acme.module.test&quot;
         * </pre>
         * 
         * @see #IMPORT_PACKAGE
         * @since 1.3
         */
        public const string BUNDLE_SYMBOLICNAME_ATTRIBUTE = "bundle-symbolic-name";

        /**
         * Manifest header directive identifying the resolution type in the
         * Import-Package or Require-Bundle manifest header. The default value is
         * {@link #RESOLUTION_MANDATORY mandatory}.
         * 
         * <p>
         * The directive value is encoded in the Import-Package or Require-Bundle
         * manifest header like:
         * 
         * <pre>
         *     Import-Package: org.osgi.framework; resolution:=&quot;optional&quot;
         *     Require-Bundle: com.acme.module.test; resolution:=&quot;optional&quot;
         * </pre>
         * 
         * @see #IMPORT_PACKAGE
         * @see #REQUIRE_BUNDLE
         * @see #RESOLUTION_MANDATORY
         * @see #RESOLUTION_OPTIONAL
         * @since 1.3
         */
        public const string RESOLUTION_DIRECTIVE = "resolution";

        /**
         * Manifest header directive value identifying a mandatory resolution type.
         * A mandatory resolution type indicates that the import package or require
         * bundle must be resolved when the bundle is resolved. If such an import or
         * require bundle cannot be resolved, the module fails to Resolve.
         * 
         * <p>
         * The directive value is encoded in the Import-Package or Require-Bundle
         * manifest header like:
         * 
         * <pre>
         *     Import-Package: org.osgi.framework; resolution:=&quot;manditory&quot;
         *     Require-Bundle: com.acme.module.test; resolution:=&quot;manditory&quot;
         * </pre>
         * 
         * @see #RESOLUTION_DIRECTIVE
         * @since 1.3
         */
        public const string RESOLUTION_MANDATORY = "mandatory";

        /**
         * Manifest header directive value identifying an optional resolution type.
         * An optional resolution type indicates that the import or require bundle
         * is optional and the bundle may be resolved without the import or require
         * bundle being resolved. If the import or require bundle is not resolved
         * when the bundle is resolved, the import or require bundle may not be
         * resolved before the bundle is refreshed.
         * 
         * <p>
         * The directive value is encoded in the Import-Package or Require-Bundle
         * manifest header like:
         * 
         * <pre>
         *     Import-Package: org.osgi.framework; resolution:=&quot;optional&quot;
         *     Require-Bundle: com.acme.module.test; resolution:=&quot;optional&quot;
         * </pre>
         * 
         * @see #RESOLUTION_DIRECTIVE
         * @since 1.3
         */
        public const string RESOLUTION_OPTIONAL = "optional";

        /**
         * Manifest header directive identifying a list of newPackages that an exported
         * package uses.
         * 
         * <p>
         * The directive value is encoded in the Export-Package manifest header
         * like:
         * 
         * <pre>
         *     Export-Package: org.osgi.util.tracker; uses:=&quot;org.osgi.framework&quot;
         * </pre>
         * 
         * @see #EXPORT_PACKAGE
         * @since 1.3
         */
        public const string USES_DIRECTIVE = "uses";

        /**
         * Manifest header directive identifying a list of classes to include in the
         * exported package.
         * 
         * <p>
         * This directive is used by the Export-Package manifest header to identify
         * a list of classes of the specified package which must be allowed to be
         * exported. The directive value is encoded in the Export-Package manifest
         * header like:
         * 
         * <pre>
         *     Export-Package: org.osgi.framework; include:=&quot;MyClass*&quot;
         * </pre>
         * 
         * <p>
         * This directive is also used by the Bundle-ActivationPolicy manifest
         * header to identify the newPackages from which class loads will trigger lazy
         * activation. The directive value is encoded in the Bundle-ActivationPolicy
         * manifest header like:
         * 
         * <pre>
         *     Bundle-ActivationPolicy: lazy; include:=&quot;org.osgi.framework&quot;
         * </pre>
         * 
         * @see #EXPORT_PACKAGE
         * @see #BUNDLE_ACTIVATIONPOLICY
         * @since 1.3
         */
        public const string INCLUDE_DIRECTIVE = "include";

        /**
         * Manifest header directive identifying a list of classes to exclude in the
         * exported package..
         * <p>
         * This directive is used by the Export-Package manifest header to identify
         * a list of classes of the specified package which must not be allowed to
         * be exported. The directive value is encoded in the Export-Package
         * manifest header like:
         * 
         * <pre>
         *     Export-Package: org.osgi.framework; exclude:=&quot;*Impl&quot;
         * </pre>
         * 
         * <p>
         * This directive is also used by the Bundle-ActivationPolicy manifest
         * header to identify the newPackages from which class loads will not trigger
         * lazy activation. The directive value is encoded in the
         * Bundle-ActivationPolicy manifest header like:
         * 
         * <pre>
         *     Bundle-ActivationPolicy: lazy; exclude:=&quot;org.osgi.framework&quot;
         * </pre>
         * 
         * @see #EXPORT_PACKAGE
         * @see #BUNDLE_ACTIVATIONPOLICY
         * @since 1.3
         */
        public const string EXCLUDE_DIRECTIVE = "exclude";

        /**
         * Manifest header directive identifying names of matching attributes which
         * must be specified by matching Import-Package statements in the
         * Export-Package manifest header.
         * 
         * <p>
         * The directive value is encoded in the Export-Package manifest header
         * like:
         * 
         * <pre>
         *     Export-Package: org.osgi.framework; mandatory:=&quot;bundle-symbolic-name&quot;
         * </pre>
         * 
         * @see #EXPORT_PACKAGE
         * @since 1.3
         */
        public const string MANDATORY_DIRECTIVE = "mandatory";

        /**
         * Manifest header directive identifying the visibility of a required bundle
         * in the Require-Bundle manifest header. The default value is
         * {@link #VISIBILITY_PRIVATE private}.
         * 
         * <p>
         * The directive value is encoded in the Require-Bundle manifest header
         * like:
         * 
         * <pre>
         *     Require-Bundle: com.acme.module.test; visibility:=&quot;reexport&quot;
         * </pre>
         * 
         * @see #REQUIRE_BUNDLE
         * @see #VISIBILITY_PRIVATE
         * @see #VISIBILITY_REEXPORT
         * @since 1.3
         */
        public const string VISIBILITY_DIRECTIVE = "visibility";

        /**
         * Manifest header directive value identifying a private visibility type. A
         * private visibility type indicates that any newPackages that are exported by
         * the required bundle are not made visible on the export signature of the
         * requiring bundle.
         * 
         * <p>
         * The directive value is encoded in the Require-Bundle manifest header
         * like:
         * 
         * <pre>
         *     Require-Bundle: com.acme.module.test; visibility:=&quot;private&quot;
         * </pre>
         * 
         * @see #VISIBILITY_DIRECTIVE
         * @since 1.3
         */
        public const string VISIBILITY_PRIVATE = "private";

        /**
         * Manifest header directive value identifying a reexport visibility type. A
         * reexport visibility type indicates any newPackages that are exported by the
         * required bundle are re-exported by the requiring bundle. Any arbitrary
         * arbitrary matching attributes with which they were exported by the
         * required bundle are deleted.
         * 
         * <p>
         * The directive value is encoded in the Require-Bundle manifest header
         * like:
         * 
         * <pre>
         *     Require-Bundle: com.acme.module.test; visibility:=&quot;reexport&quot;
         * </pre>
         * 
         * @see #VISIBILITY_DIRECTIVE
         * @since 1.3
         */
        public const string VISIBILITY_REEXPORT = "reexport";

        /**
         * Manifest header directive identifying the type of the extension fragment.
         * 
         * <p>
         * The directive value is encoded in the Fragment-Host manifest header like:
         * 
         * <pre>
         *     Fragment-Host: system.bundle; extension:=&quot;framework&quot;
         * </pre>
         * 
         * @see #FRAGMENT_HOST
         * @see #EXTENSION_FRAMEWORK
         * @see #EXTENSION_BOOTCLASSPATH
         * @since 1.3
         */
        public const string EXTENSION_DIRECTIVE = "extension";

        /**
         * Manifest header directive value identifying the type of extension
         * fragment. An extension fragment type of framework indicates that the
         * extension fragment is to be loaded by the framework's class loader.
         * 
         * <p>
         * The directive value is encoded in the Fragment-Host manifest header like:
         * 
         * <pre>
         *     Fragment-Host: system.bundle; extension:=&quot;framework&quot;
         * </pre>
         * 
         * @see #EXTENSION_DIRECTIVE
         * @since 1.3
         */
        public const string EXTENSION_FRAMEWORK = "framework";

        /**
         * Manifest header directive value identifying the type of extension
         * fragment. An extension fragment type of bootclasspath indicates that the
         * extension fragment is to be loaded by the boot class loader.
         * 
         * <p>
         * The directive value is encoded in the Fragment-Host manifest header like:
         * 
         * <pre>
         *     Fragment-Host: system.bundle; extension:=&quot;bootclasspath&quot;
         * </pre>
         * 
         * @see #EXTENSION_DIRECTIVE
         * @since 1.3
         */
        public const string EXTENSION_BOOTCLASSPATH = "bootclasspath";

        /**
         * Manifest header identifying the bundle's activation policy.
         * <p>
         * The attribute value may be retrieved from the <code>Dictionary</code>
         * object returned by the <code>Bundle.Headers</code> method.
         * 
         * @since 1.4
         * @see #ACTIVATION_LAZY
         * @see #INCLUDE_DIRECTIVE
         * @see #EXCLUDE_DIRECTIVE
         */
        public const string BUNDLE_ACTIVATIONPOLICY = "Bundle-ActivationPolicy";

        /**
         * Bundle activation policy declaring the bundle must be activated when the
         * first class load is made from the bundle.
         * <p>
         * A bundle with the lazy activation policy that is started with the
         * {@link Bundle#START_ACTIVATION_POLICY START_ACTIVATION_POLICY} option
         * will wait in the {@link Bundle#STARTING STARTING} state until the first
         * class load from the bundle occurs. The bundle will then be activated
         * before the class is returned to the requester.
         * <p>
         * The activation policy value is specified as in the
         * Bundle-ActivationPolicy manifest header like:
         * 
         * <pre>
         *       Bundle-ActivationPolicy: lazy
         * </pre>
         * 
         * @see #BUNDLE_ACTIVATIONPOLICY
         * @see Bundle#Start(int)
         * @see Bundle#START_ACTIVATION_POLICY
         * @since 1.4
         */
        public const string ACTIVATION_LAZY = "lazy";

        /**
         * Framework environment property identifying the Framework version.
         * 
         * <p>
         * The value of this property may be retrieved by calling the
         * <code>BundleContext.getProperty</code> method.
         */
        public const string FRAMEWORK_VERSION = "org.osgi.framework.version";

        /**
         * Framework environment property identifying whether the Framework supports
         * framework extension bundles.
         * 
         * <p>
         * As of version 1.4, the value of this property must be <code>true</code>.
         * The Framework must support framework extension bundles.
         * 
         * <p>
         * The value of this property may be retrieved by calling the
         * <code>BundleContext.getProperty</code> method.
         * 
         * @since 1.3
         */
        public const string SUPPORTS_FRAMEWORK_EXTENSION = "org.osgi.supports.framework.extension";

        /**
         * Framework environment property identifying whether the Framework supports
         * bootclasspath extension bundles.
         * 
         * <p>
         * If the value of this property is <code>true</code>, then the Framework
         * supports bootclasspath extension bundles. The default value is
         * <code>false</code>.
         * <p>
         * The value of this property may be retrieved by calling the
         * <code>BundleContext.getProperty</code> method.
         * 
         * @since 1.3
         */
        public const string SUPPORTS_BOOTCLASSPATH_EXTENSION = "org.osgi.supports.bootclasspath.extension";

        /**
         * Framework environment property identifying whether the Framework supports
         * fragment bundles.
         * 
         * <p>
         * As of version 1.4, the value of this property must be <code>true</code>.
         * The Framework must support fragment bundles.
         * <p>
         * The value of this property may be retrieved by calling the
         * <code>BundleContext.getProperty</code> method.
         * 
         * @since 1.3
         */
        public const string SUPPORTS_FRAMEWORK_FRAGMENT = "org.osgi.supports.framework.fragment";

        /**
         * Framework environment property identifying whether the Framework supports
         * the {@link #REQUIRE_BUNDLE Require-Bundle} manifest header.
         * 
         * <p>
         * As of version 1.4, the value of this property must be <code>true</code>.
         * The Framework must support the <code>Require-Bundle</code> manifest
         * header.
         * <p>
         * The value of this property may be retrieved by calling the
         * <code>BundleContext.getProperty</code> method.
         * 
         * @since 1.3
         */
        public const string SUPPORTS_FRAMEWORK_REQUIREBUNDLE = "org.osgi.supports.framework.requirebundle";

        #region 服务属性
        /// <summary>
        /// 服务属性标识注册一个服务所对应的所有服务接口
        /// ，值必须是<code>string[]</code>
        /// </summary>
        public const string OBJECTCLASS = "objectClass";

        /// <summary>
        /// 服务属性标识一个服务注册对象的服务标识
        /// </summary>
        public const string SERVICE_ID = "service.id";

        /// <summary>
        /// 服务属性标识一个服务的持久化标识
        /// </summary>
        public const string SERVICE_PID = "service.pid";

        /// <summary>
        /// 服务属性标识的一个服务服务的等级数
        /// </summary>
        public const string SERVICE_RANKING = "service.ranking";

        /// <summary>
        /// 服务属性标识的服务提供者
        /// </summary>
        public const string SERVICE_VENDOR = "service.vendor";

        /// <summary>
        /// 服务属性标识的服务描述
        /// </summary>
        public const string SERVICE_DESCRIPTION = "service.description";
        #endregion
    }
}
