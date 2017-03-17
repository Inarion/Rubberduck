
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;
using Rubberduck.VBEditor.SafeComWrappers;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using RubberduckTests.Mocks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Rubberduck.VBEditor;
using Antlr4.Runtime;
using Rubberduck.Common;
using VbaCodeBlock = RubberduckTests.Symbols.DeclarationFinderTests_VbaBlocks;

namespace RubberduckTests.Symbols
{

    [TestClass]
    public class DeclarationFinderTests
    {

        [TestMethod]
        [TestCategory("Resolver")]
        public void DeclarationFinder_InProcedure_MethodDeclaration()
        {
            var expectedResults = new string[]
            {
                "member1",
                "adder",
                "Foo"
            };

            var moduleContent1 = VbaCodeBlock.InProcedure_MethodDeclaration_moduleContent1();

            var tdo = new AccessibilityTestsDataObject();
            AddTestSelectionCriteria(tdo, "CFirstClass", "Foo", "Function Foo() As Long");

            AddTestComponent(tdo, tdo.SelectionModuleName, moduleContent1, ComponentType.ClassModule);

            TestAccessibleDeclarations(tdo, expectedResults);
        }

        [TestMethod]
        [TestCategory("Resolver")]
        public void DeclarationFinder_InProcedure_LocalVariableReference()
        {
            var expectedResults = new string[]
            {
                "member1",
                "Foo"
            };

            var moduleContent1 = VbaCodeBlock.InProcedure_LocalVariableReference_moduleContent1();

            var tdo = new AccessibilityTestsDataObject();
            AddTestSelectionCriteria(tdo, "modTest", "adder", "member1 + adder");
            AddTestComponent(tdo, tdo.SelectionModuleName, moduleContent1, ComponentType.StandardModule);

            TestAccessibleDeclarations(tdo, expectedResults);
        }

        [TestMethod]
        [TestCategory("Resolver")]
        public void DeclarationFinder_InProcedure_MemberDeclaration()
        {
            var expectedResults = new string[]
            {
                "adder",
                "member1",
                "Foo"
            };

            var moduleContent1 = VbaCodeBlock.InProcedure_MemberDeclaration_moduleContent1();

            var tdo = new AccessibilityTestsDataObject();
            AddTestSelectionCriteria(tdo, "CFirstClass", "member1", "member1 + adder");
            AddTestComponent(tdo, tdo.SelectionModuleName, moduleContent1, ComponentType.ClassModule);

            TestAccessibleDeclarations(tdo, expectedResults);
        }

        [TestMethod]
        [TestCategory("Resolver")]
        public void DeclarationFinder_ModuleScope()
        {
            var expectedResults = new string[]
            {
                "adder",
                "Foo"
            };

            var moduleContent1 = VbaCodeBlock.ModuleScope_CFirstClassContent();
            var moduleContent2 = VbaCodeBlock.ModuleScope_moduleContent2();

            var tdo = new AccessibilityTestsDataObject();
            AddTestSelectionCriteria(tdo, "CFirstClass", "member1", "member1 + adder");

            AddTestComponent(tdo, tdo.SelectionModuleName, moduleContent1, ComponentType.ClassModule);
            AddTestComponent(tdo, "modOne", moduleContent2, ComponentType.StandardModule);

            TestAccessibleDeclarations(tdo, expectedResults);
        }

        [TestMethod]
        [TestCategory("Resolver")]
        public void DeclarationFinder_PublicClassAndPublicModuleSub_RenameClassSub()
        {
            var expectedResults = new string[]
            {
                "Foo"
            };

            var moduleContent1 = VbaCodeBlock.PublicClassAndPubicModuleSub_CFirstClass();
            var moduleContent2 = VbaCodeBlock.PublicClassAndPubicModuleSub_moduleContent2();

            var tdo = new AccessibilityTestsDataObject();
            AddTestSelectionCriteria(tdo, "CFirstClass", "Foo", "Function Foo() As Long");

            AddTestComponent(tdo, tdo.SelectionModuleName, moduleContent1, ComponentType.ClassModule);
            AddTestComponent(tdo, "modOne", moduleContent2, ComponentType.StandardModule);

            TestAccessibleDeclarations(tdo, expectedResults);
        }

        [TestMethod]
        [TestCategory("Resolver")]
        public void DeclarationFinder_Module_To_ClassScope()
        {
            var expectedResults = new string[]
            {
                "Foo2",
                "Bar",
                "member11"
            };

            var moduleContent1 = VbaCodeBlock.Module_To_ClassScope_moduleContent1();
            var moduleContent2 = VbaCodeBlock.Module_To_ClassScope_CFirstClass();

            var tdo = new AccessibilityTestsDataObject();
            AddTestSelectionCriteria(tdo, "modOne", "member2", "member2 * 4");

            AddTestComponent(tdo, tdo.SelectionModuleName, moduleContent1, ComponentType.StandardModule);
            AddTestComponent(tdo, "CFirstClass", moduleContent2, ComponentType.ClassModule);

            TestAccessibleDeclarations(tdo, expectedResults);
        }

        [TestMethod]
        [TestCategory("Resolver")]
        public void DeclarationFinder_PrivateSub_RespectPublicSubInOtherModule()
        {
            var expectedResults = new string[]
            {
                "DoThis",
                "filename",
                "member1"
            };

            var moduleContent1 = VbaCodeBlock.PrivateSub_RespectPublicSubInOtherModule_moduleContent1();
            var moduleContent2 = VbaCodeBlock.PrivateSub_RespectPublicSubInOtherModule_moduleContent2();

            var tdo = new AccessibilityTestsDataObject();
            AddTestSelectionCriteria(tdo, "modOne", "SetFilename", "SetFilename filename");

            AddTestComponent(tdo, tdo.SelectionModuleName, moduleContent1, ComponentType.StandardModule);
            AddTestComponent(tdo, "modTwo", moduleContent2, ComponentType.StandardModule);

            TestAccessibleDeclarations(tdo, expectedResults);
        }

        [TestMethod]
        [TestCategory("Resolver")]
        public void DeclarationFinder_PrivateSub_MultipleReferences()
        {
            var expectedResults = new string[]
            {
                "DoThis",
                "filename",
                "member1",
                "StoreFilename",
                "ExtractFilename",
                "mFolderpath",
                "filepath"
            };

            var moduleContent1 = VbaCodeBlock.PrivateSub_MultipleReferences_moduleContent1();
            var moduleContent2 = VbaCodeBlock.PrivateSub_MultipleReferences_moduleContent2();
            var moduleContent3 = VbaCodeBlock.PrivateSub_MultipleReferences_moduleContent3();

            var tdo = new AccessibilityTestsDataObject();
            AddTestSelectionCriteria(tdo, "modOne", "SetFilename", "SetFilename filename");

            AddTestComponent(tdo, tdo.SelectionModuleName, moduleContent1, ComponentType.StandardModule);
            AddTestComponent(tdo, "modTwo", moduleContent2, ComponentType.StandardModule);
            AddTestComponent(tdo, "modThree", moduleContent3, ComponentType.StandardModule);

            TestAccessibleDeclarations(tdo, expectedResults);
        }

        [TestMethod]
        [TestCategory("Resolver")]
        public void DeclarationFinder_PrivateSub_WithBlock()
        {
            var expectedResults = new string[]
            {
                "mFolderpath",
                "ExtractFilename",
                "SetFilename",
                "filename",
                "input",
                "Bar"
            };

            var moduleContent1 = VbaCodeBlock.PrivateSub_WithBlock_ModuleContent1();
            var moduleContent2 = VbaCodeBlock.PrivateSub_WithBlock_CFileHelperContent();

            var tdo = new AccessibilityTestsDataObject();
            AddTestSelectionCriteria(tdo, "modOne", "StoreFilename", ".StoreFilename filepath");

            AddTestComponent(tdo, tdo.SelectionModuleName, moduleContent1, ComponentType.StandardModule);
            AddTestComponent(tdo, "CFileHelper", moduleContent2, ComponentType.ClassModule);

            TestAccessibleDeclarations(tdo, expectedResults);
        }

        [TestMethod]
        [TestCategory("Resolver")]
        public void DeclarationFinder_Module_To_ModuleScopeResolution()
        {
            var expectedResults = new string[]
            {
                "Foo1",
                "Foo2",
                "Foo3",
                "Foo4",
                "gConstant",
                "member2"
            };

            var moduleContent1 = VbaCodeBlock.Module_To_ModuleScopeResolution__moduleContent1();
            var moduleContent2 = VbaCodeBlock.Module_To_ModuleScopeResolution__moduleContent2();

            var tdo = new AccessibilityTestsDataObject();
            AddTestSelectionCriteria(tdo, "modOne", "Foo2", "Foo2 + modTwo.Foo3");

            AddTestComponent(tdo, tdo.SelectionModuleName, moduleContent1, ComponentType.StandardModule);
            AddTestComponent(tdo, "modTwo", moduleContent2, ComponentType.StandardModule);

            TestAccessibleDeclarations(tdo, expectedResults);
        }

        private void TestAccessibleDeclarations(AccessibilityTestsDataObject tdo, string[] testSpecificExpectedResults)
        {

            PrepareScenarioTestData(tdo, testSpecificExpectedResults);

            var target = tdo.Parser.AllUserDeclarations.FindTarget(tdo.QualifiedSelection);
            if(null == target)  { Assert.Inconclusive("Unable to find target from QualifiedSelection"); }

            var accessibleNames = tdo.Parser.DeclarationFinder.GetDeclarationsWithIdentifiersToAvoid(target)
                    .Select(d => d.IdentifierName);

            Assert.IsFalse(accessibleNames.Except(tdo.ExpectedResults).Any()
                    || tdo.ExpectedResults.Except(accessibleNames).Any()
                        , BuildIdentifierListToDisplay(accessibleNames.Except(tdo.ExpectedResults), tdo.ExpectedResults.Except(accessibleNames)));
        }

        private void PrepareScenarioTestData(AccessibilityTestsDataObject tdo, string[] testSpecificExpectedResults)
        {
            SetExpectedResults(tdo, testSpecificExpectedResults);

            tdo.VBE = BuildProject(tdo.ProjectName, tdo.Components);
            tdo.Parser = MockParser.CreateAndParse(tdo.VBE);

            CreateQualifiedSelectionForTestCase(tdo);
        }

        private void SetExpectedResults(AccessibilityTestsDataObject tdo, string[] testSpecificExpectedResults)
        {
            tdo.ExpectedResults = new List<string>();
            tdo.ExpectedResults.AddRange(testSpecificExpectedResults);

            //Add module name(s) and project name
            tdo.ExpectedResults.Add(tdo.SelectionTarget);
            tdo.Components.ForEach(c => tdo.ExpectedResults.Add(c.Name));
            tdo.ExpectedResults.Add(tdo.ProjectName);
        }

        private string BuildIdentifierListToDisplay(IEnumerable<string> extraIdentifiers, IEnumerable<string> missedIdentifiers)
        {
            var extraNamesPreface = "Returned unexpected identifier(s): ";
            var missedNamesPreface = "Did not return expected identifier(s): ";
            string extraResults = string.Empty;
            string missingResults = string.Empty;
            if (extraIdentifiers.Any())
            {
                extraResults = extraNamesPreface + GetListOfNames(extraIdentifiers);
            }
            if (missedIdentifiers.Any())
            {
                missingResults = missedNamesPreface + GetListOfNames(missedIdentifiers);
            }

            return "\r\n" + extraResults + "\r\n" + missingResults;
        }

        private string GetListOfNames(IEnumerable<string> identifiers)
        {
            if (!identifiers.Any()) { return ""; }

            string result = string.Empty;
            string postPend = "', ";
            foreach (var identifier in identifiers)
            {
                result = result + "'" + identifier + postPend;
            }
            return result.Remove(result.Length - postPend.Length + 1);
        }

        private void CreateQualifiedSelectionForTestCase(AccessibilityTestsDataObject tdo)
        {
            var component = RetrieveComponent(tdo, tdo.SelectionModuleName);
            var moduleContent = component.CodeModule.GetLines(1, component.CodeModule.CountOfLines);

            var splitToken = new string[] { "\r\n" };

            var lines = moduleContent.Split(splitToken, System.StringSplitOptions.None);
            int lineOfInterestNumber = 0;
            string lineOfInterestContent = string.Empty;
            for (int idx = 0; idx < lines.Count() && lineOfInterestNumber < 1; idx++)
            {
                if (lines[idx].Contains(tdo.SelectionLineIdentifier))
                {
                    lineOfInterestNumber = idx + 1;
                    lineOfInterestContent = lines[idx];
                }
            }
            Assert.IsTrue(lineOfInterestNumber > 0, "Unable to find target '" + tdo.SelectionTarget + "' in " + tdo.SelectionModuleName + " content.");
            var column = lineOfInterestContent.IndexOf(tdo.SelectionLineIdentifier);
            column = column + tdo.SelectionLineIdentifier.IndexOf(tdo.SelectionTarget) + 1;

            var moduleParent = component.CodeModule.Parent;
            tdo.QualifiedSelection = new QualifiedSelection(new QualifiedModuleName(moduleParent), new Selection(lineOfInterestNumber, column, lineOfInterestNumber, column));
        }

        private void AddTestComponent(AccessibilityTestsDataObject tdo, string moduleIdentifier, string moduleContent, ComponentType componentType)
        {
            if (null == tdo.Components)
            {
                tdo.Components = new List<TestComponentSpecification>();
            }
            tdo.Components.Add(new TestComponentSpecification(moduleIdentifier, moduleContent, componentType));
        }

        private IVBE BuildProject(string projectName, List<TestComponentSpecification> testComponents)
        {
            var builder = new MockVbeBuilder();
            var enclosingProjectBuilder = builder.ProjectBuilder(projectName, ProjectProtection.Unprotected);

            testComponents.ForEach(c => enclosingProjectBuilder.AddComponent(c.Name, c.ModuleType, c.Content));
            var enclosingProject = enclosingProjectBuilder.Build();
            builder.AddProject(enclosingProject);
            return builder.Build().Object;
        }

        private IVBComponent RetrieveComponent(AccessibilityTestsDataObject tdo, string componentName)
        {
            var vbProject = tdo.VBE.VBProjects.Where(item => item.Name == tdo.ProjectName).SingleOrDefault();
            return vbProject.VBComponents.Where(item => item.Name == componentName).SingleOrDefault();
        }

        private void AddTestSelectionCriteria(AccessibilityTestsDataObject tdo, string moduleName, string selectionTarget, string selectionLineIdentifier)
        {
            tdo.SelectionModuleName = moduleName;
            tdo.SelectionTarget = selectionTarget;
            tdo.SelectionLineIdentifier = selectionLineIdentifier;
        }

        internal class TestComponentSpecification
        {
            private string _name;
            private string _content;
            private ComponentType _componentType;
            public TestComponentSpecification(string componentName, string componentContent, ComponentType componentType)
            {
                _name = componentName;
                _content = componentContent;
                _componentType = componentType;
            }

            public string Name { get { return _name; } }
            public string Content { get { return _content; } }
            public ComponentType ModuleType { get { return _componentType; } }
        }


        internal class AccessibilityTestsDataObject
        {
            public AccessibilityTestsDataObject()
            {
                ProjectName = "TestProject";
            }
            public IVBE VBE { get; set; }
            public RubberduckParserState Parser { get; set; }
            public List<TestComponentSpecification> Components { get; set; }
            public string ProjectName { get; set; }
            public string SelectionModuleName { get; set; }
            public string SelectionTarget { get; set; }
            public string SelectionLineIdentifier { get; set; }
            public List<string> ExpectedResults { get; set; }
            public QualifiedSelection QualifiedSelection { get; set; }
        }


        [TestMethod]
        [Ignore] // ref. https://github.com/rubberduck-vba/Rubberduck/issues/2330
        public void FiendishlyAmbiguousNameSelectsSmallestScopedDeclaration()
        {
            var code = VbaCodeBlock.FiendishlyAmbiguousNameSelectsSmallestScopedDeclaration();

            var vbe = new MockVbeBuilder()
                .ProjectBuilder("foo", ProjectProtection.Unprotected)
                .AddComponent("foo", ComponentType.StandardModule, code, new Selection(6, 6))
                .MockVbeBuilder()
                .Build();

            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(vbe.Object));
            parser.Parse(new CancellationTokenSource());

            var expected = parser.State.AllDeclarations.Single(item => item.DeclarationType == DeclarationType.Variable);
            var actual = parser.State.DeclarationFinder.FindSelectedDeclaration(vbe.Object.ActiveCodePane);

            Assert.AreEqual(expected, actual, "Expected {0}, resolved to {1}", expected.DeclarationType, actual.DeclarationType);
        }

        [TestMethod]
        [Ignore] // bug: this test should pass... it's not all that evil
        public void AmbiguousNameSelectsSmallestScopedDeclaration()
        {
            var code = VbaCodeBlock.AmbiguousNameSelectsSmallestScopedDeclaration();

            var vbe = new MockVbeBuilder()
                .ProjectBuilder("TestProject", ProjectProtection.Unprotected)
                .AddComponent("TestModule", ComponentType.StandardModule, code, new Selection(6, 6))
                .MockVbeBuilder()
                .Build();

            var parser = MockParser.Create(vbe.Object, new RubberduckParserState(vbe.Object));
            parser.Parse(new CancellationTokenSource());

            var expected = parser.State.AllDeclarations.Single(item => item.DeclarationType == DeclarationType.Variable);
            var actual = parser.State.DeclarationFinder.FindSelectedDeclaration(vbe.Object.ActiveCodePane);

            Assert.AreEqual(expected, actual, "Expected {0}, resolved to {1}", expected.DeclarationType, actual.DeclarationType);
        }

        [TestCategory("Resolver")]
        [TestMethod]
        public void DeclarationFinderCanCopeWithMultipleModulesImplementingTheSameInterface()
        {
            var project = GetTestProject("testProject");
            var interf = GetTestClassModule(project, "interface");
            var member = GetTestFunction(interf, "testMember", Accessibility.Public);
            var implementingClass1 = GetTestClassModule(project, "implementingClass1");
            var implementingClass2 = GetTestClassModule(project, "implementingClass2");
            var implementsContext1 = new VBAParser.ImplementsStmtContext(null, 0);
            var implementsContext2 = new VBAParser.ImplementsStmtContext(null, 0);
            AddReference(interf, implementingClass1, implementsContext1);
            AddReference(interf, implementingClass1, implementsContext2);
            var declarations = new List<Declaration> { interf, member, implementingClass1, implementingClass2 };

            DeclarationFinder finder = new DeclarationFinder(declarations, new List<Rubberduck.Parsing.Annotations.IAnnotation>(), new List<UnboundMemberDeclaration>());
            var interfaceDeclarations = finder.FindAllInterfaceMembers().ToList();

            Assert.AreEqual(1, interfaceDeclarations.Count());
        }

        private static ClassModuleDeclaration GetTestClassModule(Declaration projectDeclatation, string name, bool isExposed = false)
        {
            var qualifiedClassModuleMemberName = new QualifiedMemberName(StubQualifiedModuleName(name), name);
            var classModuleAttributes = new Rubberduck.Parsing.VBA.Attributes();
            if (isExposed)
            {
                classModuleAttributes.AddExposedClassAttribute();
            }
            return new ClassModuleDeclaration(qualifiedClassModuleMemberName, projectDeclatation, name, false, null, classModuleAttributes);
        }

        private static ProjectDeclaration GetTestProject(string name)
        {
            var qualifiedProjectName = new QualifiedMemberName(StubQualifiedModuleName("proj"), name);
            return new ProjectDeclaration(qualifiedProjectName, name, false, null);
        }

        private static QualifiedModuleName StubQualifiedModuleName(string name)
        {
            return new QualifiedModuleName("dummy", "dummy", name);
        }

        private static FunctionDeclaration GetTestFunction(Declaration moduleDeclatation, string name, Accessibility functionAccessibility)
        {
            var qualifiedFunctionMemberName = new QualifiedMemberName(moduleDeclatation.QualifiedName.QualifiedModuleName, name);
            return new FunctionDeclaration(qualifiedFunctionMemberName, moduleDeclatation, moduleDeclatation, "test", null, "test", functionAccessibility, null, Selection.Home, false, false, null, null);
        }

        private static void AddReference(Declaration toDeclaration, Declaration fromModuleDeclaration, ParserRuleContext context = null)
        {
            toDeclaration.AddReference(toDeclaration.QualifiedName.QualifiedModuleName, fromModuleDeclaration, fromModuleDeclaration, context, toDeclaration.IdentifierName, toDeclaration, Selection.Home, new List<Rubberduck.Parsing.Annotations.IAnnotation>());
        }
    }
}