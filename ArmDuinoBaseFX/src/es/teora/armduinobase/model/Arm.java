package es.teora.armduinobase.model;

public class Arm {
	
    private int BaseAng;
    private int Horizontal1Ang;
    private int Vertical1Ang;
    private int Horizontal2Ang;
    private int Vertical2Ang;
    private int Horizontal3Ang;
    private int Grip;
    private int[] CurrentAngles;

    public int getBaseAng() {
		return BaseAng;
	}

	public void setBaseAng(int baseAng) {
		BaseAng = baseAng;
	}

	public int getHorizontal1Ang() {
		return Horizontal1Ang;
	}

	public void setHorizontal1Ang(int horizontal1Ang) {
		Horizontal1Ang = horizontal1Ang;
	}

	public int getVertical1Ang() {
		return Vertical1Ang;
	}

	public void setVertical1Ang(int vertical1Ang) {
		Vertical1Ang = vertical1Ang;
	}

	public int getHorizontal2Ang() {
		return Horizontal2Ang;
	}

	public void setHorizontal2Ang(int horizontal2Ang) {
		Horizontal2Ang = horizontal2Ang;
	}

	public int getVertical2Ang() {
		return Vertical2Ang;
	}

	public void setVertical2Ang(int vertical2Ang) {
		Vertical2Ang = vertical2Ang;
	}

	public int getHorizontal3Ang() {
		return Horizontal3Ang;
	}

	public void setHorizontal3Ang(int horizontal3Ang) {
		Horizontal3Ang = horizontal3Ang;
	}

	public int getGripAng() {
		return Grip;
	}

	public void setGripAng(int pinza) {
		Grip = pinza;
	}

	public int[] getCurrentAngles() {
		return CurrentAngles;
	}

	public void setCurrentAngles(int[] currentAngles) {
		CurrentAngles = currentAngles;
	}

	public Arm()
    {
        CurrentAngles = new int[7];
        BaseAng = 90;
        Horizontal1Ang = 90;
        Vertical1Ang = 90;
        Horizontal2Ang = 90;
        Vertical2Ang = 90;
        Horizontal3Ang = 90;
        Grip = 170;
        CurrentAngles[0] = BaseAng;
        CurrentAngles[1] = Horizontal1Ang;
        CurrentAngles[2] = Vertical1Ang;
        CurrentAngles[3] = Horizontal2Ang;
        CurrentAngles[4] = Vertical2Ang;
        CurrentAngles[5] = Horizontal3Ang;
        CurrentAngles[6] = Grip;
    }

    public void updateAngles()
    {
        CurrentAngles[0] = BaseAng;
        CurrentAngles[1] = Horizontal1Ang;
        CurrentAngles[2] = Vertical1Ang;
        CurrentAngles[3] = Horizontal2Ang;
        CurrentAngles[4] = Vertical2Ang;
        CurrentAngles[5] = Horizontal3Ang;
        CurrentAngles[6] = Grip;
    }

    public void setAngles()
    {
        BaseAng = CurrentAngles[0];
        Horizontal1Ang = CurrentAngles[1];
        Vertical1Ang = CurrentAngles[2];
        Horizontal2Ang = CurrentAngles[3];
        Vertical2Ang = CurrentAngles[4];
        Horizontal3Ang = CurrentAngles[5];
        Grip = CurrentAngles[6];
    }

}
