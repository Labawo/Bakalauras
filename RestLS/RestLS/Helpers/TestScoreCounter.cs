using System.Text.RegularExpressions;

namespace RestLS.Helpers;

public static class TestScoreCounter
{
    private static object[] SetTest()
    {
        object[] questionsData = new []
        {
            new 
            {
                question = "1. I feel tense or 'wound up':",
                options = new []
                {
                    new { text = "Most of the time", points = 3 },
                    new { text = "A lot of the time", points = 2 },
                    new { text = "From time to time, occasionally", points = 1 },
                    new { text = "Not at all", points = 0 }
                }
            },
            new 
            {
                question = "2. I still enjoy the things I used to enjoy:",
                options = new []
                {
                    new { text = "Definitely as much", points = 0 },
                    new { text = "Not quite so much", points = 1 },
                    new { text = "Only a little", points = 2 },
                    new { text = "Hardly at all", points = 3 }
                }
            },
            new 
            {
                question = "3. I get a sort of frightened feeling as if something awful is about to happen:",
                options = new []
                {
                    new { text = "Very definitely and quite badly", points = 3 },
                    new { text = "Yes, but not too badly", points = 2 },
                    new { text = "A little, but it doesn't worry me", points = 1 },
                    new { text = "Not at all", points = 0 }
                }
            },
            new 
            {
                question = "4. I can laugh and see the funny side of things::",
                options = new []
                { 
                    new { text = "As much as I always could", points = 0 },
                    new { text = "Not quite so much now ", points = 1 },
                    new { text = "Definitely not so much now", points = 2 },
                    new { text = "Not at all", points = 3 }
                }
            },
            new 
            {
                question = "5. Worrying thoughts go through my mind:",
                options = new []
                { 
                    new { text = "A great deal of the time", points = 3 },
                    new { text = "A lot of time", points = 2 },
                    new { text = "From time to time, but not too often", points = 1 },
                    new { text = "Only occasionally", points = 0 }
                }
            },
            new 
            {
                question = "6. I feel cheerful:",
                options = new []
                {
                    new { text = "Not at all", points = 3 },
                    new { text = "Not often", points = 2 },
                    new { text = "Sometimes", points = 1 },
                    new { text = "Most of the time", points = 0 }
                }
            },
            new 
            {
                question = "7. I can sit at ease and feel relaxed:",
                options = new []
                {
                    new { text = "Definitely", points = 0 },
                    new { text = "Usually", points = 1 },
                    new { text = "Not Often", points = 2 },
                    new { text = "Not at all", points = 3 }
                }
            },
            new 
            {
                question = "8. I feel as if I am slowed down:",
                options = new []
                {
                    new { text = "Nearly all the time", points = 3 },
                    new { text = "Very often", points = 2 },
                    new { text = "Sometimes", points = 1 },
                    new { text = "Not at all", points = 0 }
                }
            },
            new 
            {
                question = "9. I get a sort of frightened feeling like 'butterflies' in the stomach:",
                options = new []
                {
                    new { text = "Not at all", points = 0 },
                    new { text = "Occasionally", points = 1 },
                    new { text = "Quite Often", points = 2 },
                    new { text = "Very Often", points = 3 }
                }
            },
            new 
            {
                question = "10. I have lost interest in my appearance:",
                options = new []
                {
                    new { text = "Definitely", points = 3 },
                    new { text = "I don't take as much care as I should", points = 2 },
                    new { text = "I may not take quite as much care", points = 1 },
                    new { text = "I take just as much care as ever", points = 0 }
                }
            },
            new 
            {
                question = "11. I feel restless as I have to be on the move:",
                options = new []
                {
                    new { text = "Very much indeed", points = 3 },
                    new { text = "Quite a lot", points = 2 },
                    new { text = "Not very much", points = 1 },
                    new { text = "Not at all", points = 0 }
                }
            },
            new 
            {
                question = "12. I look forward with enjoyment to things:",
                options = new []
                {
                    new { text = "As much as I ever did", points = 0 },
                    new { text = "Rather less than I used to", points = 1 },
                    new { text = "Definitely less than I used to", points = 2 },
                    new { text = "Hardly at all", points = 3 }
                }
            },
            new 
            {
                question = "13. I get sudden feelings of panic:",
                options = new []
                {
                    new { text = "Very often indeed", points = 3 },
                    new { text = "Quite often", points = 2 },
                    new { text = "Not very often", points = 1 },
                    new { text = "Not at all", points = 0 }
                }
            },
            new 
            {
                question = "14. I can enjoy a good book or radio or TV program:",
                options = new []
                {
                    new { text = "Often", points = 0 },
                    new { text = "Sometimes", points = 1 },
                    new { text = "Not often", points = 2 },
                    new { text = "Very seldom", points = 3 }
                }
            },
        };

        return questionsData;
    }

    public static List<FinalResult> GetFinalResult(string score)
    {
        List<FinalResult> result = new List<FinalResult>();

        var questionsData = SetTest();
        
        var depressionScore = CalculateScore(score, true, questionsData);
        var anxietyScore = CalculateScore(score, false, questionsData);
        var depressionResults = FormResult(score, true, questionsData);
        var anxietyResults = FormResult(score, false, questionsData);
        
        result.Add(new FinalResult(depressionScore, depressionResults));
        result.Add(new FinalResult(anxietyScore, anxietyResults));

        return result;
    }
    
    private static int CalculateScore(string score, bool isDepression, object[] questionsData)
    {
        int result = -1;

        if (Regex.IsMatch(score, @"^\d+$"))
        {
            char[] charArray = new char[(score.Length + 1) / 2];
        
            for (int i = 0, j = 0; i < score.Length; i += 2, j++)
            {
                charArray[j] = score[i];
            }
        
            int[] intArray = new int[charArray.Length];
            for (int i = 0; i < charArray.Length; i++)
            {
                intArray[i] = charArray[i] - '0';
            }

            int sum = 0;

            for (int i = 0, j = isDepression ? 1 : 0; i < intArray.Length; i++, j+=2)
            {
                var questionData = (dynamic)questionsData[j];
                var options = (object[])questionData.options;
                
                int selectedOptionIndex = intArray[i];
                var selectedOption = (dynamic)options[selectedOptionIndex];

                sum += selectedOption.points;
            }

            result = sum;
        }
        
        return result;
    }
    
    private static string FormResult(string score, bool isDepression, object[] questionsData)
    {
        string result = "";
        
        if (Regex.IsMatch(score, @"^\d+$"))
        {
            char[] charArray = new char[(score.Length + 1) / 2];
        
            for (int i = 0, j = 0; i < score.Length; i += 2, j++)
            {
                charArray[j] = score[i];
            }
        
            int[] intArray = new int[charArray.Length];
            for (int i = 0; i < charArray.Length; i++)
            {
                intArray[i] = charArray[i] - '0';
            }

            int sum = 0;

            for (int i = 0, j = isDepression ? 1 : 0; i < intArray.Length; i++, j+=2)
            {
                var questionData = (dynamic)questionsData[j];
                result += questionData.question + "\n";
                var options = (object[])questionData.options;
                
                int selectedOptionIndex = intArray[i];
                var selectedOption = (dynamic)options[selectedOptionIndex];
                result += selectedOption.text + "\n";
                sum += selectedOption.points;
            }

            string emotion = isDepression ? "Depression" : "Anxiety";

            result += "Patients " + emotion + " Score: " + sum + " " + CheckScoreConclusion(sum) + ".";

            return result;
        }
        
        return null;
    }

    private static string CheckScoreConclusion(int score)
    {
        var result = "undefined";
        if (score >= 0 && score <= 7)
        {
            result = "Normal";
        }
        else if (score > 8 && score <= 10)
        {
            result = "Borderline abnormal (borderline case)";
        }
        else if (score > 11 && score <= 21)
        {
            result = "Abnormal (case)";
        }

        return result;
    }
}

