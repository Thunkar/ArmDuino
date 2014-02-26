package es.teora.armduinobase.controller;
import es.teora.armduinobase.model.Arm;
import es.teora.armduinobase.model.COMHandler;


public class MainController {
	
	public static MainController Current;
	public COMHandler COMHandler;
	public Arm CurrentArm;
	
	public MainController() 
	{
		Current = this;
		CurrentArm = new Arm();
		COMHandler = new COMHandler(CurrentArm);
	}
	


}
