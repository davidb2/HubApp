package main;

import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.GridLayout;
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
import javax.swing.JTextField;
import javax.swing.ButtonGroup;
import javax.swing.JButton;

public class QuestionDialog extends JFrame {
	private JTextArea jta;
	private JTextField questionarea, answer1, answer2, answer3, answer4, timer; 
	private int delay;
	private boolean waiting;
	public static void main(String[] args){
		new QuestionDialog();
	}
	private void showMessage(String message){
		jta.setText(jta.getText()+"\n"+message);
	}
	private void outputMessage(String message){
		try {
			FileWriter f = new FileWriter(new File("output.txt"));
			f.write(message);
			f.close();
		} catch (IOException e) {}
	}
	public QuestionDialog(){
		waiting = false;
		setLayout(new BorderLayout());
		JLabel title = new JLabel("Ask a Question");
		title.setFont(title.getFont().deriveFont(25f));
		add(title,BorderLayout.PAGE_START);
		JPanel resultsPanel = new JPanel();
		resultsPanel.setLayout(new BoxLayout(resultsPanel, BoxLayout.Y_AXIS));
			jta = new JTextArea(15,15);
			JScrollPane jsp = new JScrollPane(jta);
			resultsPanel.add(jsp);
		add(resultsPanel, BorderLayout.LINE_END);
		JPanel selectPanel = new JPanel();
		selectPanel.setLayout(new BoxLayout(selectPanel,BoxLayout.Y_AXIS));
			ButtonGroup modes = new ButtonGroup();
			JRadioButton auto = new JRadioButton("Automatic");
			modes.add(auto);
			selectPanel.add(auto);
			JRadioButton manual = new JRadioButton("Manual");
			modes.add(manual);
			selectPanel.add(manual);
			modes.setSelected(manual.getModel(), true);
			selectPanel.add(Box.createRigidArea(new Dimension(0,50)));
			JLabel jl = new JLabel("Set Timer");
			selectPanel.add(jl);
			JTextField jtf = new JTextField("");
			selectPanel.add(jtf);
			JButton confTimer = new JButton("Confirm");
			confTimer.addActionListener(new ActionListener(){
				public void actionPerformed(ActionEvent e){
					if(!waiting){
						try{
							int d = Integer.parseInt(jtf.getText());
							showMessage("Delay set to: "+d+"s");
							delay = d;
						}catch(NumberFormatException ne){showMessage("Invalid delay time.");jtf.setText("");}
					}else{showMessage("Waiting on previous request");}
				}
			});
			selectPanel.add(confTimer);
			selectPanel.add(Box.createRigidArea(new Dimension(0,20)));
			JLabel timelabel = new JLabel("Until Answer:");
			selectPanel.add(timelabel);
			timer = new JTextField("   0:00");
			selectPanel.add(timer);
			selectPanel.add(Box.createRigidArea(new Dimension(0,30)));
		add(selectPanel, BorderLayout.LINE_START);
		JPanel mainPanel = new JPanel();
			mainPanel.setLayout(new BoxLayout(mainPanel, BoxLayout.Y_AXIS));
			JLabel questionlabel = new JLabel("Question: ");
			mainPanel.add(questionlabel);
			questionarea = new JTextField("");
			questionarea.setSize(200,40);
			mainPanel.add(questionarea);
			mainPanel.add(Box.createRigidArea(new Dimension(300,30)));
			JLabel answerlabel = new JLabel("Answer: [CORRECT FIRST]");
			mainPanel.add(answerlabel);
			answer1 = new JTextField("");
			answer1.setSize(200,40);
			mainPanel.add(answer1);
			answer2 = new JTextField("");
			answer2.setSize(200,40);
			mainPanel.add(answer2);
			answer3 = new JTextField("");
			answer3.setSize(200,40);
			mainPanel.add(answer3);
			answer4 = new JTextField("");
			answer4.setSize(200,40);
			mainPanel.add(answer4);
			JButton confquestion = new JButton("Submit");
			confquestion.addActionListener(new ActionListener(){
				public void actionPerformed(ActionEvent e){
					Thread t = new Thread(){
						public void run(){
							try{
								waiting = true;
								showMessage(questionarea.getText());
								outputMessage(questionarea.getText()+'\n'+answer1.getText()+'\n'+answer2.getText()+'\n'+answer3.getText()+'\n'+answer4.getText());
								for(int i=delay-1;i>=0;i--){
									String seconds = "0"+(i%60);
									timer.setText((i/60)+":"+seconds.substring(seconds.length()-2,seconds.length()));
									Thread.sleep(1000);
								}
								waiting = false;
								showMessage(answer1.getText());
								outputMessage(answer1.getText());
							}catch(InterruptedException ie){waiting = false;showMessage("Thread Error");}
						}
					};
					if(!waiting){t.start();}
					else{showMessage("Waiting on previous request");}
				}
			});
			mainPanel.add(confquestion);
		add(mainPanel, BorderLayout.CENTER);
		setSize(700,400);
		setVisible(true);
	}
}
