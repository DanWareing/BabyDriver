using System.Collections.Generic;

namespace BabyDriver
{
    public static class Dialogue
    {

        public static List<string> docPositive = new List<string>{
            "Doc: Slow down, I'll contact you soon.",
            "Doc: Sit tight and wait.",
            "Doc: I've got something in the works. It'll be with you soon."
        };

        public static List<string> notEnoughSeats = new List<string>
        {
            "Crew: C'mon man, obviously we can't all fit in there. Piss off and find a bigger car.",
            "Crew: Can you count? Go and get a bigger whip.",
            "Crew: Seriously? Is one of us getting in the trunk? Go and rob a car with enough seats."
        };

        public static List<string> notStolen = new List<string>
        {
            "Crew: Wait up, isn't that your ride? Go and steal someone else's.",
            "Crew: You can't do a job in your own car. Go get another!",
            "Crew: You gotta steal a ride before we go anywhere."
        };

        public static List<string> wantedOnArrival = new List<string>
        {
            "Crew: Oh shit! The cops followed us here! Drive!",
            "Crew: We've been followed. Bail!",
            "Crew: The fuckin' cops! Get us out of here!"
        };

        public static List<string> stageCloseFail = new List<string>
        {
            "Crew: That was too close. Be more cautious next time, or you'll get us all busted.",
            "Crew: Nice getaway, but you gotta be more careful next time.",
            "Crew: We gotta lose them before we get back to the lockup next time. Laters."
        };

        public static List<string> docMissionReady = new List<string>
        {
            "Okay, we're ready. Drop by.",
            "Let's meet. I've got a job.",
            "Alright, let's get to work."
        };

        public static List<string> stageEndSuccess = new List<string>
        {
            "I believe congratulations are due. Let's work together again."
        };

        public static List<string> stageEndFail = new List<string>
        {
            "You've just made some very powerful enemies. Never contact me again, and sleep with one eye open..."
        };

    }
}
