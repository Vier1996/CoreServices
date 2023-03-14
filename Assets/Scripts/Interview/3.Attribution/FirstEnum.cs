namespace Interview._3.Attribution
{
    public enum FirstEnum
    {
        [FromFirstToSecondConnect(SecondEnum.COLD)] NONE = 0,
        [FromFirstToSecondConnect(SecondEnum.COLD)] RED = 1,
        [FromFirstToSecondConnect(SecondEnum.COLD)] GREEN = 2,
        [FromFirstToSecondConnect(SecondEnum.COLD)] BLUE = 3,
    }
}