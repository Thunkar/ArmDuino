package es.teora.armduinobase.model;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.OutputStream;


public class COMHandler {
	
	
	 public static SerialPort Port;
     public boolean isConnected = false;
     public Arm currentArm;

     public COMHandler(Arm currentArm)
     {
         this.currentArm = currentArm;
     }

     public void Initialize()
     {
         String[] portnames = SerialPort.GetPortNames();

         

         Port = new SerialPort("COM6", 115200);
         while (!Port.IsOpen)
         {
             try
             {
                 Port.Open();
             }
             catch (Exception e)
             {
            	 e.printStackTrace();
             }
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
             Port.Write(buffer, 0, buffer.length);
         }
         catch(Exception e)
         {
             e.printStackTrace()
         }
     }

     public void writeDataString()
     {
         Port.WriteLine(dataToString());
     }
	


}
