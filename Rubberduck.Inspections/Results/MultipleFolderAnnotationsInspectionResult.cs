using Antlr4.Runtime;
using Rubberduck.Common;
using Rubberduck.Inspections.Abstract;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Parsing.Inspections.Resources;
using Rubberduck.VBEditor;

namespace Rubberduck.Inspections.Results
{
    public class MultipleFolderAnnotationsInspectionResult : InspectionResultBase
    {
        public MultipleFolderAnnotationsInspectionResult(IInspection inspection, QualifiedContext<ParserRuleContext> context, QualifiedMemberName? qualifiedName) 
            : base(inspection, context.ModuleName, qualifiedName, context.Context) {}

        public override string Description
        {
            get { return string.Format(InspectionsUI.MultipleFolderAnnotationsInspectionResultFormat, QualifiedName.ComponentName).Capitalize(); }
        }
    }
}