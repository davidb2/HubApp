package hubbapp;

import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;

import javax.swing.Box;
import javax.swing.BoxLayout;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JRadioButton;
import javax.swing.JScrollPane;
import javax.swing.JTextArea;
import javax.swing.ButtonGroup;
import javax.swing.JButton;

public class QuestionDialog extends JFrame {
	
	private static final long serialVersionUID = 1494074271554998681L;
	private static final int SLEEP_TIME = 1000;
	private static final int SECONDS_IN_MINUTE = 60;
	private static final String OUTPUT_FILE = "output.txt";
	private JTextArea jta, questionarea, answerarea, timer, jtf, answer1, answer2, answer3, answer4;
	private int delay;
	private boolean waiting;
	
	public static void main(String[] args) {
		new QuestionDialog();
	}
	
	private void showMessage(String message) {
		jta.setText(String.format("%s\n%s", jta.getText(), message));
	}
	
	private void outputMessage(String message) {
		try {
			FileWriter file = new FileWriter(new File(OUTPUT_FILE));
			file.write(message);
			file.close();
		} catch (IOException e) {
			System.err.println(e);
		}
	}
	
	private void initializeOptions() {
		waiting = false;
		setLayout(new BorderLayout());
	}
	
	private void initializeTitle() {
		JLabel title = new JLabel("Ask a Question");
		
		title.setFont(title.getFont().deriveFont(25f));
		
		add(title, BorderLayout.PAGE_START);
	}
	
	private void initializeResultsPanel() {
		jta                 = new JTextArea(15, 15);
		JPanel resultsPanel = new JPanel();
		JScrollPane jsp     = new JScrollPane(jta);
		
		resultsPanel.setLayout(new BoxLayout(resultsPanel, BoxLayout.Y_AXIS));
		resultsPanel.add(jsp);
		
		add(resultsPanel, BorderLayout.LINE_END);
	}
	
	private void initializeSelectPanel() {
		timer               = new JTextArea("   0:00");
		jtf                 = new JTextArea(""); 
		JPanel selectPanel  = new JPanel();
		ButtonGroup modes   = new ButtonGroup();
		JRadioButton auto   = new JRadioButton("Automatic");
		JRadioButton manual = new JRadioButton("Manual");
		JLabel jl           = new JLabel("Set Timer");
		JButton confTimer   = new JButton("Confirm");
		JLabel timelabel    = new JLabel("Until Answer:");
		
		confTimer.addActionListener(e -> confirmTimer_onClick(e));
		
		selectPanel.setLayout(new BoxLayout(selectPanel, BoxLayout.Y_AXIS));
		
		selectPanel.add(auto);
		selectPanel.add(manual);
		selectPanel.add(Box.createRigidArea(new Dimension(0, 50)));
		selectPanel.add(jl);
		selectPanel.add(jtf);
		selectPanel.add(confTimer);
		selectPanel.add(Box.createRigidArea(new Dimension(0, 20)));
		selectPanel.add(timelabel);
		selectPanel.add(timer);
		selectPanel.add(Box.createRigidArea(new Dimension(0, 30)));
		
		modes.add(auto);
		modes.add(manual);
		modes.setSelected(manual.getModel(), true);

		add(selectPanel, BorderLayout.LINE_START);
	}
	
	private void initializeMainPanel() {
		questionarea = new JTextArea("");
		answerarea = new JTextArea("");
		JPanel mainPanel = new JPanel();
		JLabel questionlabel = new JLabel("Question: ");
		JLabel answerlabel = new JLabel("Answer: ");
		JButton confquestion = new JButton("Submit");
		
		questionarea.setSize(200, 40);
		answerarea.setSize(200, 40);
		
		mainPanel.setLayout(new BoxLayout(mainPanel, BoxLayout.Y_AXIS));
		mainPanel.add(questionlabel);
		mainPanel.add(questionarea);
		mainPanel.add(Box.createRigidArea(new Dimension(300, 30)));
		mainPanel.add(answerlabel);
		mainPanel.add(answerarea);
		
		confquestion.addActionListener(e -> confirmQuestion_onClick(e));
		
		mainPanel.add(confquestion);
		
		add(mainPanel, BorderLayout.CENTER);
	}
	
	private void initializeWindowSettings() {
		this.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		setSize(700, 400);
		setVisible(true);
	}
	
	private void confirmTimer_onClick(ActionEvent e) {
		if (!waiting) {
			try {
				int d = Integer.parseInt(jtf.getText());
				showMessage(String.format("Delay set to: %ds", d));
				delay = d;
			} catch (NumberFormatException ne) {
				showMessage("Invalid delay time.");
				jtf.setText("");
			}
		} else {
			showMessage("Waiting on previous request");
		}
	}

	private void confirmQuestion_onClick(ActionEvent e) {
		Thread thread = new Thread(() -> countdownTimer());
		
		if (!waiting) {
			thread.start();
		} else {
			showMessage("Waiting on previous request");
		}
	}
	
	private void countdownTimer() {
		try {
			waiting = true;
			showMessage(questionarea.getText());
			outputMessage(questionarea.getText());
			
			for (int i = delay - 1; i >= 0; i--) {
				String time = String.format("%d:%02d", i / SECONDS_IN_MINUTE, i % SECONDS_IN_MINUTE);
				timer.setText(time);
				Thread.sleep(SLEEP_TIME);
			}
			waiting = false;
			showMessage(answerarea.getText());
			outputMessage(answerarea.getText());
		} catch (InterruptedException ie) {
			waiting = false;
			showMessage("Thread Error");
		}
	}
	
	public QuestionDialog() {
		initializeOptions();
		initializeTitle();
		initializeResultsPanel();
		initializeSelectPanel();
		initializeMainPanel();
		initializeWindowSettings();
	}
}