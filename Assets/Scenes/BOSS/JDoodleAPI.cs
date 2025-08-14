using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Text;
using SimpleJSON;

public class JDoodleAPI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField codeInput;
    public TMP_Text outputText;

    private string clientId = "73bcee3a86445db55595e75d8186f29b";
    private string clientSecret = "7d1ebeaf3d882d36715e298b7af66f83496065b1c2c01839899d4c5eef92f781";
    private const string jdoodleEndpoint = "https://api.jdoodle.com/v1/execute";

    public void RunCode()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            outputText.text = "No internet connection. Please connect and try again.";
            return;
        }

        string userCode = codeInput.text;

        string fullCode = "public class Main {\n"
                        + "    public static void main(String[] args) {\n"
                        + userCode + "\n"
                        + "    }\n"
                        + "}";

        StartCoroutine(SendToJDoodle(fullCode));
    }

    public IEnumerator EvaluateCode(string userCode, System.Action<string> callback)
    {
        string fullCode = "public class Main {\n"
                        + "    public static void main(String[] args) {\n"
                        + userCode + "\n"
                        + "    }\n"
                        + "}";

        string jsonData = "{\"clientId\":\"" + clientId + "\"," +
                          "\"clientSecret\":\"" + clientSecret + "\"," +
                          "\"script\":\"" + EscapeJson(fullCode) + "\"," +
                          "\"language\":\"java\"," +
                          "\"versionIndex\":\"4\"}";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(jdoodleEndpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string output = "";
        if (request.result != UnityWebRequest.Result.Success)
        {
            output = request.error;
        }
        else
        {
            var json = JSON.Parse(request.downloadHandler.text);
            output = json["output"];
        }

        callback(output);
    }

    private IEnumerator SendToJDoodle(string userCode)
    {
        outputText.text = "Running code...";

        string jsonData = "{\"clientId\":\"" + clientId + "\"," +
                          "\"clientSecret\":\"" + clientSecret + "\"," +
                          "\"script\":\"" + EscapeJson(userCode) + "\"," +
                          "\"language\":\"java\"," +
                          "\"versionIndex\":\"4\"}";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(jdoodleEndpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            outputText.text = $"Request failed:\n{request.error}";
        }
        else
        {
            var json = JSON.Parse(request.downloadHandler.text);
            string rawOutput = json["output"];
            outputText.text = AdjustLineNumbers(rawOutput).Trim();
        }
    }

    public void RunCodeWithExpectedOutput(string userCode, string expectedOutput, System.Action<bool, bool> callback, string requiredKeyword = "")
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            outputText.text = "No internet connection. Please connect and try again.";
            callback(false, false); // Second bool indicates API failure
            return;
        }

        string fullCode = "public class Main {\n"
                        + "    public static void main(String[] args) {\n"
                        + userCode + "\n"
                        + "    }\n"
                        + "}";

        StartCoroutine(SendToJDoodleWithValidation(fullCode, expectedOutput, callback, requiredKeyword));
    }

    private IEnumerator SendToJDoodleWithValidation(string fullCode, string expectedOutput, System.Action<bool, bool> callback, string requiredKeyword)
    {
        string jsonData = "{\"clientId\":\"" + clientId + "\"," +
                          "\"clientSecret\":\"" + clientSecret + "\"," +
                          "\"script\":\"" + EscapeJson(fullCode) + "\"," +
                          "\"language\":\"java\"," +
                          "\"versionIndex\":\"4\"}";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest request = new UnityWebRequest(jdoodleEndpoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            outputText.text = $"Request failed:\n{request.error}";
            callback(false, false); // API failure
            yield break;
        }

        try
        {
            var json = JSON.Parse(request.downloadHandler.text);
            if (json == null || json.IsNull || !json.HasKey("output"))
            {
                outputText.text = "Invalid response from JDoodle API";
                callback(false, false); // API failure
                yield break;
            }

            string rawOutput = json["output"];
            string cleanExpected = expectedOutput.Trim();
            string cleanActual = rawOutput.Trim();

            bool outputCorrect = cleanExpected == cleanActual;
            bool keywordUsed = string.IsNullOrEmpty(requiredKeyword) || fullCode.Contains(requiredKeyword);

            if (outputCorrect)
            {
                StartCoroutine(ShowOutputTemporarily(rawOutput.Trim(), 3f));
            }
            else
            {
                outputText.text = SimplifyJDoodleError(rawOutput);
            }

            callback(outputCorrect && keywordUsed, true); // Second bool indicates API success
        }
        catch (System.Exception e)
        {
            outputText.text = $"Error processing response: {e.Message}";
            callback(false, false); // API failure
        }
    }

    private IEnumerator ShowOutputTemporarily(string output, float duration)
    {
        outputText.text = output;
        yield return new WaitForSeconds(duration);
        outputText.text = "";
    }

    private string EscapeJson(string s)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in s)
        {
            switch (c)
            {
                case '\\': sb.Append("\\\\"); break;
                case '"': sb.Append("\\\""); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append(""); break;
                case '\t': sb.Append("\\t"); break;
                default:
                    if (char.IsControl(c))
                        sb.AppendFormat("\\u{0:X4}", (int)c);
                    else
                        sb.Append(c);
                    break;
            }
        }
        return sb.ToString();
    }

    private string AdjustLineNumbers(string jdoodleOutput)
    {
        if (string.IsNullOrEmpty(jdoodleOutput))
            return jdoodleOutput;

        string[] lines = jdoodleOutput.Split('\n');
        StringBuilder adjusted = new StringBuilder();

        System.Text.RegularExpressions.Regex lineNumberRegex = new System.Text.RegularExpressions.Regex(
            @"(?:(?<file>\w+\.java):|:)(?<line>\d+):|\(line (?<line2>\d+)\)");

        foreach (string line in lines)
        {
            var match = lineNumberRegex.Match(line);
            if (match.Success)
            {
                string lineStr = match.Groups["line"].Success ? match.Groups["line"].Value : match.Groups["line2"].Value;

                if (int.TryParse(lineStr, out int reportedLine))
                {
                    int userLine = Mathf.Max(reportedLine - 2, 1);
                    string newLine = lineNumberRegex.Replace(
                        line,
                        m => m.Groups["file"].Success
                            ? $"Line {userLine}:"
                            : m.Groups["line2"].Success
                                ? $"(Line {userLine})"
                                : $"Line {userLine}:",
                        1);

                    adjusted.AppendLine(newLine);
                    continue;
                }
            }

            adjusted.AppendLine(line);
        }

        return adjusted.ToString();
    }

    public string SimplifyJDoodleError(string rawOutput)
    {
        if (string.IsNullOrWhiteSpace(rawOutput))
            return "No error message found.";

        StringBuilder simplified = new StringBuilder();
        string[] lines = rawOutput.Split('\n');
        int totalErrors = 0;

        for (int i = 0; i < lines.Length - 1; i++)
        {
            if (lines[i].Contains("error:"))
            {
                string errorLine = lines[i];
                string offendingLine = lines[i + 1];
                string markerLine = (i + 2 < lines.Length && lines[i + 2].Trim().StartsWith("^")) ? lines[i + 2] : "";

                string message = errorLine.Substring(errorLine.IndexOf("error:") + "error:".Length).Trim();

                simplified.AppendLine($"Error: {message}");
                simplified.AppendLine($"{offendingLine}");
                if (!string.IsNullOrEmpty(markerLine))
                    simplified.AppendLine($"   {markerLine}");

                simplified.AppendLine();
                totalErrors++;
            }
        }

        // Check for ending summary like "3 errors"
        string lastLine = lines[lines.Length - 1];
        if (lastLine.Contains("error"))
        {
            simplified.AppendLine($"Total Errors: {totalErrors}");
        }

        return simplified.ToString().Trim();
    }


}
