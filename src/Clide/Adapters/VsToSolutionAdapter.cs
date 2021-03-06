﻿using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Composition;

namespace Clide
{

    [Adapter]
    internal class VsToSolutionAdapter :
        IAdapter<IVsHierarchy, IProjectNode>,
        IAdapter<FlavoredProject, IProjectNode>
    {
        readonly Lazy<ISolutionExplorerNodeFactory> nodeFactory;
        readonly JoinableLazy<IVsHierarchyItemManager> hierarchyItemManager;

        [ImportingConstructor]
        public VsToSolutionAdapter(
            Lazy<ISolutionExplorerNodeFactory> nodeFactory,
            JoinableLazy<IVsHierarchyItemManager> hierarchyItemManager)
        {
            this.nodeFactory = nodeFactory;
            this.hierarchyItemManager = hierarchyItemManager;
        }

        public IProjectNode Adapt(IVsHierarchy from) =>
            nodeFactory
                .Value
                .CreateNode(hierarchyItemManager.GetValue().GetHierarchyItem(from, VSConstants.VSITEMID_ROOT))
                as IProjectNode;

        public IProjectNode Adapt(FlavoredProject from) =>
            (nodeFactory
                .Value
                .CreateNode(hierarchyItemManager.GetValue().GetHierarchyItem(from.Hierarchy, VSConstants.VSITEMID_ROOT))
                as ProjectNode).WithInnerHierarchy(hierarchyItemManager.GetValue().GetHierarchyItem(from.InnerHierarchy, VSConstants.VSITEMID_ROOT));
    }
}