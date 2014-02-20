package es.teora.armduinobase.model;

public class Arm {
	
    private int BaseAng;
    private int Horizontal1Ang;
    private int Vertical1Ang;
    private int Horizontal2Ang;
    private int Vertical2Ang;
    private int Horizontal3Ang;
    private int Pinza;
    public int[] CurrentAngles;

    public Arm()
    {
        CurrentAngles = new int[7];
        BaseAng = 90;
        Horizontal1Ang = 90;
        Vertical1Ang = 90;
        Horizontal2Ang = 90;
        Vertical2Ang = 90;
        Horizontal3Ang = 90;
        Pinza = 170;
        CurrentAngles[0] = BaseAng;
        CurrentAngles[1] = Horizontal1Ang;
        CurrentAngles[2] = Vertical1Ang;
        CurrentAngles[3] = Horizontal2Ang;
        CurrentAngles[4] = Vertical2Ang;
        CurrentAngles[5] = Horizontal3Ang;
        CurrentAngles[6] = Pinza;
    }

    public void updateAngles()
    {
        CurrentAngles[0] = BaseAng;
        CurrentAngles[1] = Horizontal1Ang;
        CurrentAngles[2] = Vertical1Ang;
        CurrentAngles[3] = Horizontal2Ang;
        CurrentAngles[4] = Vertical2Ang;
        CurrentAngles[5] = Horizontal3Ang;
        CurrentAngles[6] = Pinza;
    }

    public void setAngles()
    {
        BaseAng = CurrentAngles[0];
        Horizontal1Ang = CurrentAngles[1];
        Vertical1Ang = CurrentAngles[2];
        Horizontal2Ang = CurrentAngles[3];
        Vertical2Ang = CurrentAngles[4];
        Horizontal3Ang = CurrentAngles[5];
        Pinza = CurrentAngles[6];
    }

}
