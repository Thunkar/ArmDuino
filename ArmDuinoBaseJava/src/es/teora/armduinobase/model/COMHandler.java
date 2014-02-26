package es.teora.armduinobase.model;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.util.Enumeration;
import gnu.io.*;


public class COMHandler implements SerialPortEventListener{



	public boolean isConnected = false;
	public Arm currentArm;
	SerialPort Port;
	/** The port we're normally going to use. */
	private static final String PORT_NAMES[] = { 
		"/dev/tty.usbserial-A9007UX1", // Mac OS X
		"/dev/ttyUSB0", // Linux
		"COM3", // Windows
	};
	/**
	 * A BufferedReader which will be fed by a InputStreamReader 
	 * converting the bytes into characters 
	 * making the displayed results codepage independent
	 */
	private BufferedReader input;
	/** The output stream to the port */
	private OutputStream output;
	/** Milliseconds to block while waiting for port open */
	private static final int TIME_OUT = 2000;
	/** Default bits per second for COM port. */
	private static final int DATA_RATE = 115200;

	public COMHandler(Arm currentArm)
	{
		this.currentArm = currentArm;

	}

	public void Initialize()
	{
		CommPortIdentifier portId = null;
		Enumeration portEnum = CommPortIdentifier.getPortIdentifiers();

		//First, Find an instance of serial port as set in PORT_NAMES.
		while (portEnum.hasMoreElements()) {
			CommPortIdentifier currPortId = (CommPortIdentifier) portEnum.nextElement();
			for (String portName : PORT_NAMES) {
				if (currPortId.getName().equals(portName)) {
					portId = currPortId;
					break;
				}
			}
		}
		if (portId == null) {
			System.out.println("Could not find COM port.");
			return;
		}

		try {
			// open serial port, and use class name for the appName.
			Port = (SerialPort) portId.open(this.getClass().getName(),
					TIME_OUT);

			// set port parameters
			Port.setSerialPortParams(DATA_RATE,
					SerialPort.DATABITS_8,
					SerialPort.STOPBITS_1,
					SerialPort.PARITY_NONE);

			// open the streams
			input = new BufferedReader(new InputStreamReader(Port.getInputStream()));
			output = Port.getOutputStream();

			// add event listeners
			Port.addEventListener(this);
			Port.notifyOnDataAvailable(true);
		} catch (Exception e) {
			System.err.println(e.toString());
		}
	}
	
	/**
	 * Handle an event on the serial port. Read the data and print it.
	 */
	public synchronized void serialEvent(SerialPortEvent oEvent) {
		if (oEvent.getEventType() == SerialPortEvent.DATA_AVAILABLE) {
			try {
				String inputLine=input.readLine();
				System.out.println(inputLine);
			} catch (Exception e) {
				System.err.println(e.toString());
			}
		}
		// Ignore all the other eventTypes, but you should consider the other ones.
	}
	
	/**
	 * This should be called when you stop using the port.
	 * This will prevent port locking on platforms like Linux.
	 */
	public synchronized void close() {
		if (Port != null) {
			Port.removeEventListener();
			Port.close();
		}
	}


	public String dataToString()
	{
		String result = "";
		for (int i = 0; i < currentArm.CurrentAngles.length; i++)
		{
			if (currentArm.CurrentAngles[i] < 10)
			{
				result += "00" + currentArm.CurrentAngles[i];
			}
			else if (currentArm.CurrentAngles[i] < 100)
			{
				result += "0" + currentArm.CurrentAngles[i];
			}
			else result += currentArm.CurrentAngles[i];
		}
		return result;
	}

	public byte[] dataToBytes()
	{
		char[] result = dataToString().toCharArray();
		byte[] buffer = new byte[21];
		for (int i = 0; i < result.length; i++)
		{
			buffer[i] = (byte)Integer.parseInt(""+result[i]);
		}
		return buffer;
	}

	public void writeDataBytes()
	{
		try
		{
			byte[] buffer = dataToBytes();
			output.write(buffer, 0, buffer.length);
		}
		catch(Exception e)
		{
			e.printStackTrace();
		}
	}
}
