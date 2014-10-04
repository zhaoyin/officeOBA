using System;
using System.Collections;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义插件启动级别管理器
    /// </summary>
    internal class StartLevelManager : IStartLevel
    {
        private static readonly Logger Log = new Logger(typeof(StartLevelManager));
        /** The initial bundle start level for newly installed bundles */
        private int initialBundleStartLevel = 1;
        // default value is 1 for compatibility mode

        /** The currently active framework start level */
        private int activeSL = 0;

        /** An object used to lock the active startlevel while it is being referenced */
        private object lockObj = new object();
        private Framework framework;

        /** This constructor is called by the Framework */
        public StartLevelManager(Framework framework)
        {
            this.framework = framework;
        }

        internal void initialize()
        {
            //initialBundleStartLevel = framework.adaptor.getInitialBundleStartLevel();
        }

        internal void cleanup()
        {

        }

        /// <summary>
        /// 获取或设置插件启动时的初始启动级别
        /// </summary>
        public int InitialBundleStartLevel
        {
            get { return initialBundleStartLevel; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException();
                }
                initialBundleStartLevel = value;
                //framework.adaptor.setInitialBundleStartLevel(startlevel);
            }
        }


        /// <summary>
        /// 设置启动级别
        /// </summary>
        public int StartLevel
        {
            get { return activeSL; }
            set
            {
                this.SetStartLevel(value, null);
            }
        }


        public void SetStartLevel(int newSL, AbstractBundle callerBundle)
        {

            if (newSL <= 0)
            {
                throw new ArgumentException(""); //$NON-NLS-1$ 
            }
        }

        /**
         *  Internal method to shut down the framework synchronously by setting the startlevel to zero
         *  and calling the StartLevelListener worker calls directly
         *
         *  This method does not return until all bundles are stopped and the framework is shut down.
         */
        internal void Shutdown()
        {
            doSetStartLevel(0);
        }

        /**
         *  Internal worker method to set the startlevel
         *
         * @param newSL start level value                  
         * @param callerBundle - the bundle initiating the change in start level
         */
        internal void doSetStartLevel(int newSL)
        {
            lock (lockObj)
            {
                //ClassLoader previousTCCL = Thread.currentThread().getContextClassLoader();
                //ClassLoader contextFinder = framework.getContextFinder();
                //if (contextFinder == previousTCCL)
                //    contextFinder = null;
                //else
                //    Thread.currentThread().setContextClassLoader(contextFinder);
                try
                {
                    int tempSL = activeSL;
                    if (newSL > tempSL)
                    {
                        bool launching = tempSL == 0;
                        for (int i = tempSL; i < newSL; i++)
                        {
                            tempSL++;
                            // Note that we must get a new list of installed bundles each time;
                            // this is because additional bundles could have been installed from the previous start-level
                            //incFWSL(i + 1, GetInstalledBundles(framework.bundles, false));
                        }
                        if (launching)
                        {
                            //framework.systemBundle.state = Bundle.ACTIVE;
                            //framework.publishBundleEvent(BundleEvent.STARTED, framework.systemBundle);
                            //framework.publishFrameworkEvent(FrameworkEvent.STARTED, framework.systemBundle, null);
                        }
                    }
                    else
                    {
                        //AbstractBundle[] sortedBundles = GetInstalledBundles(framework.bundles, true);
                        //for (int i = tempSL; i > newSL; i--)
                        //{
                        //    tempSL--;
                        //    decFWSL(i - 1, sortedBundles);
                        //}
                        //if (newSL == 0)
                        //{
                        //    // stop and unload all bundles
                        //    suspendAllBundles(framework.bundles);
                        //    unloadAllBundles(framework.bundles);
                        //}
                    }
                    //framework.publishFrameworkEvent(FrameworkEvent.STARTLEVEL_CHANGED, framework.systemBundle, null);
                }
                finally
                {
                    //if (contextFinder != null)
                    //    Thread.currentThread().setContextClassLoader(previousTCCL);
                }
            }
        }

        /** 
         * This method is used within the package to save the actual active startlevel value for the framework.
         * Externally the setStartLevel method must be used.
         * 
         * @param newSL - the new startlevel to save
         */
        internal void SaveActiveStartLevel(int newSL)
        {
            lock (lockObj)
            {
                activeSL = newSL;
            }
        }

        /**
         * Return the persistent state of the specified bundle.
         *
         * <p>This method returns the persistent state of a bundle.
         * The persistent state of a bundle indicates whether a bundle
         * is persistently marked to be started when it's start level is
         * reached.
         *
         * @return <tt>true</tt> if the bundle is persistently marked to be started,
         * <tt>false</tt> if the bundle is not persistently marked to be started.
         * @exception java.lang.IllegalArgumentException If the specified bundle has been uninstalled.
         */
        public bool IsBundlePersistentlyStarted(AbstractBundle bundle)
        {
            //return (((AbstractBundle) bundle).BundleDataBundleData().getStatus() & Constants.BUNDLE_STARTED) != 0;
            return false;
        }

        public bool IsBundleActivationPolicyUsed(AbstractBundle bundle)
        {
            //return (((AbstractBundle) bundle).getBundleData().getStatus() & Constants.BUNDLE_ACTIVATION_POLICY) != 0;
            return false;
        }

        /**
         * Return the assigned start level value for the specified Bundle.
         *
         * @param bundle The target bundle.
         * @return The start level value of the specified Bundle.
         * @exception java.lang.IllegalArgumentException If the specified bundle has been uninstalled.
         */
        public int GetBundleStartLevel(AbstractBundle bundle)
        {

            return ((AbstractBundle)bundle).StartLevel;
        }

        /**
         * Assign a start level value to the specified Bundle.
         *
         * <p>The specified bundle will be assigned the specified start level. The
         * start level value assigned to the bundle will be persistently recorded
         * by the Framework.
         *
         * If the new start level for the bundle is lower than or equal to the active start level of
         * the Framework, the Framework will start the specified bundle as described
         * in the <tt>Bundle.start</tt> method if the bundle is persistently marked
         * to be started. The actual starting of this bundle must occur asynchronously.
         *
         * If the new start level for the bundle is higher than the active start level of
         * the Framework, the Framework will stop the specified bundle as described
         * in the <tt>Bundle.stop</tt> method except that the persistently recorded
         * state for the bundle indicates that the bundle must be restarted in the
         * future. The actual stopping of this bundle must occur asynchronously.
         *
         * @param bundle The target bundle.
         * @param newSL The new start level for the specified Bundle.
         * @throws IllegalArgumentException
         * If the specified bundle has been uninstalled or
         * if the specified start level is less than or equal to zero, or the  specified bundle is
         * the system bundle.
         * @throws SecurityException if the caller does not have the
         * <tt>AdminPermission</tt> and the Java runtime environment supports
         * permissions.
         */
        public void SetBundleStartLevel(AbstractBundle bundle, int newSL)
        {

            String exceptionText = null;
            if (bundle.BundleId == 0)
            { // system bundle has id=0
                exceptionText = "不能改变插件的启动级别";
            }
            else if (newSL <= 0)
            {
                exceptionText = "启动级别非法"; //$NON-NLS-1$ 
            }
            if (exceptionText != null)
                throw new ArgumentException(exceptionText);
            try
            {
                // if the bundle's startlevel is not already at the requested startlevel
                if (newSL != ((AbstractBundle)bundle).StartLevel)
                {
                    AbstractBundle b = bundle as AbstractBundle;
                    b.BundleData.StartLevel = (newSL);
                    try
                    {
                       // b.BundleData.Save();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    // handle starting or stopping the bundle asynchronously
                    //issueEvent(new StartLevelEvent(StartLevelEvent.CHANGE_BUNDLE_SL, newSL, (AbstractBundle) bundle));
                }
            }
            catch (Exception e)
            {
                Log.Debug(e);
                //framework.publishFrameworkEvent(FrameworkEvent.ERROR, bundle, e);
            }

        }


        /** 
         *  Increment the active startlevel by one
         */
        internal void incFWSL(int incToSL, AbstractBundle[] launchBundles)
        {
            // save the startlevel
            SaveActiveStartLevel(incToSL);
            // resume all bundles at the startlevel
            resumeBundles(launchBundles, incToSL);
        }

        /**
         * Build an array of all installed bundles to be launch.
         * The returned array is sorted by increasing startlevel/id order.
         * @param bundles - the bundles installed in the framework
         * @return A sorted array of bundles 
         */
        internal AbstractBundle[] GetInstalledBundles(BundleRepository bundles, bool sortByDependency)
        {

            /* make copy of bundles vector in case it is modified during launch */
            AbstractBundle[] installedBundles;

            lock (bundles)
            {
                IList allBundles = bundles.GetBundles();
                installedBundles = new AbstractBundle[allBundles.Count];
                allBundles.CopyTo(installedBundles, 0);

                /* Sort bundle array in ascending startlevel / bundle id order
                 * so that bundles are started in ascending order.
                 */
                BundleUtil.Sort(installedBundles, 0, installedBundles.Length);
                //if (sortByDependency)
                //    SortByDependency(installedBundles);
            }
            return installedBundles;
        }

        internal void SortByDependency(AbstractBundle[] bundles)
        {
            lock (framework.Bundles)
            {
                if (bundles.Length <= 1)
                    return;
                int currentSL = bundles[0].StartLevel;
                int currentSLindex = 0;
                bool lazy = false;
                for (int i = 0; i < bundles.Length; i++)
                {
                    if (currentSL != bundles[i].StartLevel)
                    {
                        if (lazy)
                            SortByDependencies(bundles, currentSLindex, i);
                        currentSL = bundles[i].StartLevel;
                        currentSLindex = i;
                        lazy = false;
                    }
                    lazy |= (bundles[i].BundleData.Policy == ActivatorPolicy.Lazy);
                }
                // Sort the last set of bundles
                if (lazy)
                    SortByDependencies(bundles, currentSLindex, bundles.Length);
            }
        }

        private void SortByDependencies(AbstractBundle[] bundles, int start, int end)
        {
            //if (end - start <= 1)
            //    return;
            //IList descList = new ArrayList(end - start);
            //IList missingDescs = new ArrayList(0);
            //for (int i = start; i < end; i++)
            //{
            //    IBundleDescription desc = bundles[i].getBundleDescription();
            //    if (desc != null)
            //        descList.add(desc);
            //    else
            //        missingDescs.add(bundles[i]);
            //}
            //if (descList.size() <= 1)
            //    return;
            //BundleDescription[] descriptions = (BundleDescription[])descList.toArray(new BundleDescription[descList.size()]);
            //framework.adaptor.getPlatformAdmin().getStateHelper().sortBundles(descriptions);
            //for (int i = start; i < descriptions.length + start; i++)
            //    bundles[i] = framework.bundles.getBundle(descriptions[i - start].getBundleId());
            //if (missingDescs.size() > 0)
            //{
            //    Iterator missing = missingDescs.iterator();
            //    for (int i = start + descriptions.length; i < end && missing.hasNext(); i++)
            //        bundles[i] = (AbstractBundle)missing.next();
            //}
        }

        /**
         *  Resume all bundles in the launch list at the specified start-level
         * @param launch a list of Bundle Objects to launch
         * @param currentSL the current start-level that the bundles must meet to be resumed
         */
        private void resumeBundles(AbstractBundle[] launch, int currentSL)
        {
            // Resume all bundles that were previously started and whose startlevel is <= the active startlevel
            // first resume the lazy activated bundles
            resumeBundles(launch, true, currentSL);
            // now resume all non lazy bundles
            resumeBundles(launch, false, currentSL);
        }

        private void resumeBundles(AbstractBundle[] launch, bool lazyOnly, int currentSL)
        {
            //for (int i = 0; i < launch.length && !framework.isForcedRestart(); i++) {
            //    int bsl = launch[i].getStartLevel();
            //    if (bsl < currentSL) {
            //        // skip bundles who should have already been started
            //        continue;
            //    } else if (bsl == currentSL) {
            //        if (Debug.DEBUG && Debug.DEBUG_STARTLEVEL) {
            //            Debug.println("SLL: Active sl = " + currentSL + "; Bundle " + launch[i].getBundleId() + " sl = " + bsl); //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
            //        }
            //        boolean isLazyStart = launch[i].isLazyStart();
            //        if (lazyOnly ? isLazyStart : !isLazyStart)
            //            framework.resumeBundle(launch[i]);
            //    } else {
            //        // can stop resuming bundles since any remaining bundles have a greater startlevel than the framework active startlevel
            //        break;
            //    }
            //}
        }

        /** 
         *  Decrement the active startlevel by one
         * @param decToSL -  the startlevel value to set the framework to
         */
        protected void decFWSL(int decToSL, AbstractBundle[] shutdown)
        {

            SaveActiveStartLevel(decToSL);

            if (decToSL == 0) // stopping the framework
                return;

            // just decrementing the active startlevel - framework is not shutting down
            // Do not check framework.isForcedRestart here because we want to stop the active bundles regardless.
            for (int i = shutdown.Length - 1; i >= 0; i--)
            {
                int bsl = shutdown[i].StartLevel;
                if (bsl > decToSL + 1)
                    // skip bundles who should have already been stopped
                    continue;
                else if (bsl <= decToSL)
                    // stopped all bundles we are going to for this start level
                    break;
                else if (shutdown[i].IsActive)
                {
                    // if bundle is active or starting, then stop the bundle
                    //framework.SuspendBundle(Shutdown[i], false);
                }
            }
        }

        /**
         *  Suspends all bundles in the vector passed in.
         * @param bundles list of Bundle objects to be suspended
         */
        private void suspendAllBundles(BundleRepository bundles)
        {
            bool changed;
            do
            {
                changed = false;

                AbstractBundle[] shutdown = this.GetInstalledBundles(bundles, false);

                // Shutdown all running bundles
                for (int i = shutdown.Length -1; i >= 0; i--)
                {
                    AbstractBundle bundle = shutdown[i];

                    //if (framework.SuspendBundle(bundle, false)) {
                    //    changed = true;
                    //}
                }
            } while (changed);

            try
            {
                //framework.systemBundle.context.stop();
            }
            catch (BundleException sbe)
            {
                Log.Debug(sbe);
                //framework.publishFrameworkEvent(FrameworkEvent.ERROR, framework.systemBundle, sbe);
            }

            //framework.systemBundle.state = Bundle.RESOLVED;
            //framework.publishBundleEvent(BundleEvent.STOPPED, framework.systemBundle);
        }

        /** 
         *  Set the bundle's startlevel to the new value
         *  This may cause the bundle to start or stop based on the active framework startlevel
         * @param startLevelEvent - the event requesting change in bundle startlevel
         */
        //protected void setBundleSL(StartLevelEvent startLevelEvent) {
        //    lock (lockObj) {
        //        int currentSL = StartLevel;
        //        int newSL = startLevelEvent.getNewSL();
        //        AbstractBundle bundle = startLevelEvent.getBundle();

        //        if (bundle.IsActive && (newSL > currentSL)) {
        //            //framework.SuspendBundle(bundle, false);
        //        } else {
        //            if (!bundle.IsActive && (newSL <= currentSL))
        //            {
        //                //framework.resumeBundle(bundle);
        //            }
        //        }
        //    }
        //}



        public int GetBundleStartLevel(IBundle bundle)
        {
            throw new NotImplementedException();
        }

        public void SetBundleStartLevel(IBundle bundle, int startlevel)
        {
            throw new NotImplementedException();
        }

        public bool IsBundlePersistentlyStarted(IBundle bundle)
        {
            throw new NotImplementedException();
        }

        public bool IsBundleActivationPolicyUsed(IBundle bundle)
        {
            throw new NotImplementedException();
        }
    }
}
