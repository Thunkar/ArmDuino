package es.teora.armduinobase.view;
	


import java.io.IOException;

import es.teora.armduinobase.controller.MainController;
import javafx.application.Application;
import javafx.beans.value.ChangeListener;
import javafx.beans.value.ObservableValue;
import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.geometry.Orientation;
import javafx.geometry.Pos;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.Slider;
import javafx.scene.layout.GridPane;
import javafx.stage.Stage;
import javafx.stage.WindowEvent;


public class MainView extends Application {
	
	public Slider BaseSlider;
	public Slider Horizontal1Slider;
	public Slider Vertical1Slider;
	public Slider Horizontal2Slider;
	public Slider Vertical2Slider;
	public Slider Horizontal3Slider;
	public Slider GripSlider;
	public Button Connect;
	public GridPane MainGrid;
	public static MainController MainController;
	
	@Override
	public void start(Stage primaryStage) 
	{
		MainController = new MainController();
		try {
			MainGrid = new GridPane();
			MainGrid.setAlignment(Pos.CENTER);
			MainGrid.setHgap(10);
			MainGrid.setVgap(10);
			Scene scene = new Scene(MainGrid, 500,500);
			scene.getStylesheets().add(getClass().getResource("application.css").toExternalForm());
			setSliders();
			Label Title = new Label("ArmDuino Base");
			Title.setAlignment(Pos.TOP_LEFT);
			MainGrid.add(Title, 0, 0);
			primaryStage.setOnCloseRequest(new EventHandler<WindowEvent>() {
			    @Override
			    public void handle(WindowEvent event) {
			        try {
			        	MainController.COMHandler.close();
					} catch (Exception e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					}
			    }
			});
			primaryStage.setScene(scene);
			primaryStage.show();
		} catch(Exception e) {
			e.printStackTrace();
		}
	}
	
	
	public void setSliders()
	{
		Connect = new Button();
		MainGrid.add(Connect, 0, 2);
		Connect.setText("Connect");
		Connect.setOnAction(new EventHandler<ActionEvent>() {
			   @Override public void handle(ActionEvent e) {
				   byte[] init = { 7, 7, 7, 7 };
		            try {
						MainController.COMHandler.getOutput().write(init, 0, 4);
						MainController.COMHandler.currentArm.updateAngles();
						MainController.COMHandler.writeDataBytes();
					} catch (IOException e1) {
						e1.printStackTrace();
					}
			   }
			});
		BaseSlider = new Slider();
		BaseSlider.minProperty().set(0);
		BaseSlider.maxProperty().set(180);
		BaseSlider.valueProperty().set(90);
		MainGrid.add(BaseSlider, 1, 1);
		BaseSlider.orientationProperty().set(Orientation.VERTICAL);
		BaseSlider.valueProperty().addListener(new ChangeListener<Number>() {
	            public void changed(ObservableValue<? extends Number> ov,
	                Number old_val, Number new_val) 
	            {
	            		MainController.COMHandler.currentArm.setBaseAng(new_val.intValue());
	            		MainController.COMHandler.currentArm.updateAngles();
	            		MainController.COMHandler.writeDataBytes();
	            }
	        });
		Horizontal1Slider = new Slider();
		Horizontal1Slider.minProperty().set(0);
		Horizontal1Slider.maxProperty().set(180);
		Horizontal1Slider.valueProperty().set(90);
		MainGrid.add(Horizontal1Slider, 2, 1);
		Horizontal1Slider.orientationProperty().set(Orientation.VERTICAL);
		Horizontal1Slider.valueProperty().addListener(new ChangeListener<Number>() {
	            public void changed(ObservableValue<? extends Number> ov,
	                Number old_val, Number new_val) 
	            {
	            		MainController.COMHandler.currentArm.setHorizontal1Ang(new_val.intValue());
	            		MainController.COMHandler.currentArm.updateAngles();
	            		MainController.COMHandler.writeDataBytes();
	            }
	        });
		Vertical1Slider = new Slider();
		Vertical1Slider.minProperty().set(0);
		Vertical1Slider.maxProperty().set(180);
		Vertical1Slider.valueProperty().set(90);
		MainGrid.add(Vertical1Slider, 3, 1);
		Vertical1Slider.orientationProperty().set(Orientation.VERTICAL);
		Vertical1Slider.valueProperty().addListener(new ChangeListener<Number>() {
	            public void changed(ObservableValue<? extends Number> ov,
	                Number old_val, Number new_val) 
	            {
	            		MainController.COMHandler.currentArm.setVertical1Ang(new_val.intValue());
	            		MainController.COMHandler.currentArm.updateAngles();
	            		MainController.COMHandler.writeDataBytes();
	            }
	        });
		Horizontal2Slider = new Slider();
		Horizontal2Slider.minProperty().set(0);
		Horizontal2Slider.maxProperty().set(180);
		Horizontal2Slider.valueProperty().set(90);
		MainGrid.add(Horizontal2Slider, 4, 1);
		Horizontal2Slider.orientationProperty().set(Orientation.VERTICAL);
		Horizontal2Slider.valueProperty().addListener(new ChangeListener<Number>() {
	            public void changed(ObservableValue<? extends Number> ov,
	                Number old_val, Number new_val) 
	            {
	            		MainController.COMHandler.currentArm.setHorizontal2Ang(new_val.intValue());
	            		MainController.COMHandler.currentArm.updateAngles();
	            		MainController.COMHandler.writeDataBytes();
	            }
	        });
		Vertical2Slider = new Slider();
		Vertical2Slider.minProperty().set(0);
		Vertical2Slider.maxProperty().set(180);
		Vertical2Slider.valueProperty().set(90);
		MainGrid.add(Vertical2Slider, 5, 1);
		Vertical2Slider.orientationProperty().set(Orientation.VERTICAL);
		Vertical2Slider.valueProperty().addListener(new ChangeListener<Number>() {
	            public void changed(ObservableValue<? extends Number> ov,
	                Number old_val, Number new_val) 
	            {
	            		MainController.COMHandler.currentArm.setVertical2Ang(new_val.intValue());
	            		MainController.COMHandler.currentArm.updateAngles();
	            		MainController.COMHandler.writeDataBytes();
	            }
	        });
		Horizontal3Slider = new Slider();
		Horizontal3Slider.minProperty().set(0);
		Horizontal3Slider.maxProperty().set(180);
		Horizontal3Slider.valueProperty().set(90);
		MainGrid.add(Horizontal3Slider, 6, 1);
		Horizontal3Slider.orientationProperty().set(Orientation.VERTICAL);
		Horizontal3Slider.valueProperty().addListener(new ChangeListener<Number>() {
	            public void changed(ObservableValue<? extends Number> ov,
	                Number old_val, Number new_val) 
	            {
	            		MainController.COMHandler.currentArm.setHorizontal3Ang(new_val.intValue());
	            		MainController.COMHandler.currentArm.updateAngles();
	            		MainController.COMHandler.writeDataBytes();
	            }
	        });
		GripSlider = new Slider();
		MainGrid.add(GripSlider, 7, 1);
		GripSlider.minProperty().set(90);
		GripSlider.maxProperty().set(180);
		GripSlider.valueProperty().set(90);
		GripSlider.orientationProperty().set(Orientation.VERTICAL);
		GripSlider.valueProperty().addListener(new ChangeListener<Number>() {
	            public void changed(ObservableValue<? extends Number> ov,
	                Number old_val, Number new_val) 
	            {
	            		MainController.COMHandler.currentArm.setGripAng(new_val.intValue());
	            		MainController.COMHandler.currentArm.updateAngles();
	            		MainController.COMHandler.writeDataBytes();
	            }
	        });
	}
	
	public static void main(String[] args) 
	{
		launch(args);
	}
}
