package es.teora.armduinobase.model;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.OutputStream;
import gnu.io.*;


public class COMHandler implements SerialPortEventListener{


	public boolean isConnected = false;
	public Arm currentArm;
	private SerialPort Port;
	/**
	 * A BufferedReader which will be fed by a InputStreamReader 
	 * converting the bytes into characters 
	 * making the displayed results codepage independent
	 */
	private BufferedReader input;
	/** The output stream to the port */
	private OutputStream output;
	public BufferedReader getInput() {
		return input;
	}

	public void setInput(BufferedReader input) {
		this.input = input;
	}

	public OutputStream getOutput() {
		return output;
	}

	public void setOutput(OutputStream output) {
		this.output = output;
	}

	/** Milliseconds to block while waiting for port open */
	private static final int TIME_OUT = 2000;
	/** Default bits per second for COM port. */
	private static final int DATA_RATE = 115200;

	public COMHandler(Arm currentArm)
	{
		this.currentArm = currentArm;
		Initialize();

	}


	public void Initialize()
	{
		CommPortIdentifier portId = null;
		portId = (CommPortIdentifier)CommPortIdentifier.getPortIdentifiers().nextElement();
		if (portId == null) {
			System.out.println("Could not find COM port.");
			return;
		}

		try {
			// open serial port, and use class name for the appName.
			setPort((SerialPort) portId.open(this.getClass().getName(),
					TIME_OUT));

			// set port parameters
			getPort().setSerialPortParams(DATA_RATE,
					SerialPort.DATABITS_8,
					SerialPort.STOPBITS_1,
					SerialPort.PARITY_NONE);

			// open the streams
			input = new BufferedReader(new InputStreamReader(getPort().getInputStream()));
			output = getPort().getOutputStream();

			// add event listeners
			getPort().addEventListener(this);
			getPort().notifyOnDataAvailable(true);
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
	public synchronized void close() 
	{
		if (getPort() != null) 
		{
			getPort().removeEventListener();
			getPort().close();
		}
	}


	public String dataToString()
	{
		String result = "200";
		for (int i = 0; i < currentArm.getCurrentAngles().length; i++)
		{
			if (currentArm.getCurrentAngles()[i] < 10)
			{
				result += "00" + currentArm.getCurrentAngles()[i];
			}
			else if (currentArm.getCurrentAngles()[i] < 100)
			{
				result += "0" + currentArm.getCurrentAngles()[i];
			}
			else result += currentArm.getCurrentAngles()[i];
		}
		return result;
	}

	public byte[] dataToBytes()
	{
		char[] result = dataToString().toCharArray();
		byte[] buffer = new byte[24];
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

	public SerialPort getPort() {
		return Port;
	}

	public void setPort(SerialPort port) {
		Port = port;
	}
}
