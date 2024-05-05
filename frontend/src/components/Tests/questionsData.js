const questionsData = [
    {
      question: "1.",
      options: [
        { text: "I do not feel sad.", points: 0 },
        { text: "I feel sad.", points: 1 },
        { text: "I am sad all the time and I can't snap out of it.", points: 2 },
        { text: "I am so sad and unhappy that I can't stand it.", points: 3 },
      ],
    },
    {
      question: "2.",
      options: [
        { text: "I am not particularly discouraged about the future.", points: 0 },
        { text: "I feel discouraged about the future.", points: 1 },
        { text: "I feel I have nothing to look forward to.", points: 2 },
        { text: "I feel the future is hopeless and that things cannot improve.", points: 3 },
      ],
    },
    {
        question: "3.",
        options: [
          { text: "I do not feel like a failure.", points: 0 },
          { text: "I feel I have failed more than the average person.", points: 1 },
          { text: "As I look back on my life, all I can see is a lot of failures.", points: 2 },
          { text: "I feel I am a complete failure as a person.", points: 3 },
        ],
      },
      {
        question: "4.",
        options: [
          { text: "I get as much satisfaction out of things as I used to.", points: 0 },
          { text: "I don't enjoy things the way I used to.", points: 1 },
          { text: "I don't get real satisfaction out of anything anymore.", points: 2 },
          { text: "I am dissatisfied or bored with everything.", points: 3 },
        ],
      },
      {
        question: "5.",
        options: [
          { text: "I don't feel particularly guilty.", points: 0 },
          { text: "I feel guilty a good part of the time.", points: 1 },
          { text: "I feel quite guilty most of the time.", points: 2 },
          { text: "I feel guilty all of the time.", points: 3 },
        ],
      },
      {
        question: "6.",
        options: [
          { text: "I don't feel I am being punished.", points: 0 },
          { text: "I feel I may be punished.", points: 1 },
          { text: "I expect to be punished.", points: 2 },
          { text: "I feel I am being punished.", points: 3 },
        ],
      },
      {
        question: "7.",
        options: [
          { text: "I don't feel disappointed in myself.", points: 0 },
          { text: "I am disappointed in myself.", points: 1 },
          { text: "I am disgusted with myself.", points: 2 },
          { text: "I hate myself.", points: 3 },
        ],
      },
      {
        question: "8.",
        options: [
          { text: "I don't feel I am any worse than anybody else.", points: 0 },
          { text: "I am critical of myself for my weaknesses or mistakes.", points: 1 },
          { text: "I blame myself all the time for my faults.", points: 2 },
          { text: "I blame myself for everything bad that happens.", points: 3 },
        ],
      },
      {
        question: "9.",
        options: [
          { text: "I don't have any thoughts of killing myself.", points: 0 },
          { text: "I have thoughts of killing myself, but I would not carry them out.", points: 1 },
          { text: "I would like to kill myself.", points: 2 },
          { text: "I would kill myself if I had the chance.", points: 3 },
        ],
      },
      {
        question: "10.",
        options: [
          { text: "I don't cry any more than usual.", points: 0 },
          { text: "I cry more now than I used to.", points: 1 },
          { text: "I cry all the time now.", points: 2 },
          { text: "I used to be able to cry, but now I can't cry even though I want to.", points: 3 },
        ],
      },
      {
        question: "11.",
        options: [
          { text: "I am no more irritated by things than I ever was.", points: 0 },
          { text: "I am slightly more irritated now than usual.", points: 1 },
          { text: "I am quite annoyed or irritated a good deal of the time.", points: 2 },
          { text: "I feel irritated all the time.", points: 3 },
        ],
      },
      {
        question: "12.",
        options: [
          { text: "I have not lost interest in other people.", points: 0 },
          { text: "I am less interested in other people than I used to be.", points: 1 },
          { text: "I have lost most of my interest in other people.", points: 2 },
          { text: "I have lost all of my interest in other people.", points: 3 },
        ],
      },
      {
        question: "13.",
        options: [
          { text: "I make decisions about as well as I ever could.", points: 0 },
          { text: "I put off making decisions more than I used to.", points: 1 },
          { text: "I have greater difficulty in making decisions more than I used to.", points: 2 },
          { text: "I can't make decisions at all anymore.", points: 3 },
        ],
      },
      {
        question: "14.",
        options: [
          { text: "I don't feel that I look any worse than I used to.", points: 0 },
          { text: "I am worried that I am looking old or unattractive.", points: 1 },
          { text: "I feel there are permanent changes in my appearance that make me look unattractive.", points: 2 },
          { text: "I believe that I look ugly.", points: 3 },
        ],
      },
      {
        question: "15.",
        options: [
          { text: "I can work about as well as before.", points: 0 },
          { text: "It takes an extra effort to get started at doing something.", points: 1 },
          { text: "I have to push myself very hard to do anything.", points: 2 },
          { text: "I can't do any work at all.", points: 3 },
        ],
      },
      {
        question: "16.",
        options: [
          { text: "I can sleep as well as usual.", points: 0 },
          { text: "I don't sleep as well as I used to.", points: 1 },
          { text: "I wake up 1-2 hours earlier than usual and find it hard to get back to sleep.", points: 2 },
          { text: "I wake up several hours earlier than I used to and cannot get back to sleep.", points: 3 },
        ],
      },
      {
        question: "17.",
        options: [
          { text: "I don't get more tired than usual.", points: 0 },
          { text: "I get tired more easily than I used to.", points: 1 },
          { text: "I get tired from doing almost anything.", points: 2 },
          { text: "I am too tired to do anything.", points: 3 },
        ],
      },
      {
        question: "18.",
        options: [
          { text: "My appetite is no worse than usual.", points: 0 },
          { text: "My appetite is not as good as it used to be.", points: 1 },
          { text: "My appetite is much worse now.", points: 2 },
          { text: "I have no appetite at all anymore.", points: 3 },
        ],
      },
      {
        question: "19.",
        options: [
          { text: "I haven't lost much weight, if any, lately.", points: 0 },
          { text: "I have lost more than five pounds.", points: 1 },
          { text: "I have lost more than ten pounds.", points: 2 },
          { text: "I have lost more than fifteen pounds.", points: 3 },
        ],
      },
      {
        question: "20.",
        options: [
          { text: "I am no more worried about my health than usual.", points: 0 },
          { text: "I am worried about physical problems like aches, pains, upset stomach, or constipation.", points: 1 },
          { text: "I am very worried about physical problems and it's hard to think of much else.", points: 2 },
          { text: "I am so worried about my physical problems that I cannot think of anything else.", points: 3 },
        ],
      },
      {
        question: "21.",
        options: [
          { text: "I have not noticed any recent change in my interest in sex.", points: 0 },
          { text: "I am less interested in sex than I used to be.", points: 1 },
          { text: "I have almost no interest in sex.", points: 2 },
          { text: "I have lost interest in sex completely.", points: 3 },
        ],
      },
  ];
  
  export default questionsData;
  