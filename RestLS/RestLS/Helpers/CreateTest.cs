namespace RestLS.Helpers;

public class CreateTest
{
    public class HADTest
    {
        private List<HADQuestion> questions;

        public void AddQuestion(HADQuestion question)
        {
            questions.Add(question);
        }

        public HADQuestion GetQuestion(int index)
        {
            return questions[index];
        }
    }

    public class HADQuestion
    {
        private List<HADOption> options;

        public void Add(HADOption option)
        {
            options.Add(option);
        }

        public HADOption GetOption(int index)
        {
            return options[index];
        }
    }

    public class HADOption
    {
        public string Text { get; set; }
        public int Points { get; set; }
    }
}