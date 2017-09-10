using System;
using System.Reflection;

namespace INATEL_T141_DM106_Final_Homework.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}