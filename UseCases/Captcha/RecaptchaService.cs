using System;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.RecaptchaEnterprise.V1;

namespace CsharpClicker.Web.UseCases.Captcha;

public class RecaptchaService(string projectId, string recaptchaKey)
{
    public readonly string projectId = projectId;
    public readonly string recaptchaKey = recaptchaKey;

    public async Task<bool> VerifyRecaptchaAsync(string token, string action)
    {
        RecaptchaEnterpriseServiceClient client = await RecaptchaEnterpriseServiceClient.CreateAsync();
        ProjectName projectName = new ProjectName(projectId);

        var createAssessmentRequest = new CreateAssessmentRequest
        {
            ParentAsProjectName = projectName,
            Assessment = new Assessment
            {
                Event = new Event
                {
                    SiteKey = recaptchaKey,
                    Token = token,
                    ExpectedAction = action
                }
            }
        };

        var assessment = await client.CreateAssessmentAsync(createAssessmentRequest);
        return assessment.TokenProperties.Valid && assessment.RiskAnalysis.Score >= 0.5;
    }
}