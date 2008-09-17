namespace Anubis {
	partial class Anubis {
		///// <summary>
		///// Variable nécessaire au concepteur.
		///// </summary>
		//private System.ComponentModel.IContainer components = null;		

		/// <summary>
		/// Nettoyage des ressources utilisées.
		/// </summary>
		/// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}		

		#region Code généré par le Concepteur Windows Form

		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent() {
            this.components=new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources=new System.ComponentModel.ComponentResourceManager(typeof(Anubis));
            this.listenerColumnHeader=new System.Windows.Forms.ColumnHeader();
            this.listViewPlugins=new System.Windows.Forms.ListView();
            this.listViewJobs=new System.Windows.Forms.ListView();
            this.columnHeader1=new System.Windows.Forms.ColumnHeader();
            this.listViewJobsDone=new System.Windows.Forms.ListView();
            this.columnHeader2=new System.Windows.Forms.ColumnHeader();
            this.listViewJobsError=new System.Windows.Forms.ListView();
            this.columnHeader3=new System.Windows.Forms.ColumnHeader();
            this.groupBoxJobs=new System.Windows.Forms.GroupBox();
            this.nbJobslabel=new System.Windows.Forms.Label();
            this.label1=new System.Windows.Forms.Label();
            this.listViewMessage=new System.Windows.Forms.ListView();
            this.iconeColumnHeader=new System.Windows.Forms.ColumnHeader();
            this.messageColumnHeader=new System.Windows.Forms.ColumnHeader();
            this.IconsList=new System.Windows.Forms.ImageList(this.components);
            this.mainMenu=new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1=new System.Windows.Forms.MenuItem();
            this.menuItemFileExit=new System.Windows.Forms.MenuItem();
            this.anubisPictureBox=new System.Windows.Forms.PictureBox();
            this.panel=new System.Windows.Forms.Panel();
            this.groupBoxJobs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.anubisPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // listenerColumnHeader
            // 
            this.listenerColumnHeader.Text="Plug-ins";
            this.listenerColumnHeader.Width=204;
            // 
            // listViewPlugins
            // 
            this.listViewPlugins.Activation=System.Windows.Forms.ItemActivation.OneClick;
            this.listViewPlugins.Anchor=((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top|System.Windows.Forms.AnchorStyles.Bottom)
                        |System.Windows.Forms.AnchorStyles.Left)));
            this.listViewPlugins.AutoArrange=false;
            this.listViewPlugins.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.listenerColumnHeader});
            this.listViewPlugins.FullRowSelect=true;
            this.listViewPlugins.GridLines=true;
            this.listViewPlugins.HeaderStyle=System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewPlugins.Location=new System.Drawing.Point(72,23);
            this.listViewPlugins.MultiSelect=false;
            this.listViewPlugins.Name="listViewPlugins";
            this.listViewPlugins.Size=new System.Drawing.Size(208,216);
            this.listViewPlugins.TabIndex=1;
            this.listViewPlugins.UseCompatibleStateImageBehavior=false;
            this.listViewPlugins.View=System.Windows.Forms.View.Details;
            // 
            // listViewJobs
            // 
            this.listViewJobs.Activation=System.Windows.Forms.ItemActivation.OneClick;
            this.listViewJobs.Anchor=((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top|System.Windows.Forms.AnchorStyles.Bottom)
                        |System.Windows.Forms.AnchorStyles.Left)));
            this.listViewJobs.AutoArrange=false;
            this.listViewJobs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewJobs.FullRowSelect=true;
            this.listViewJobs.GridLines=true;
            this.listViewJobs.HeaderStyle=System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewJobs.Location=new System.Drawing.Point(8,24);
            this.listViewJobs.MultiSelect=false;
            this.listViewJobs.Name="listViewJobs";
            this.listViewJobs.Size=new System.Drawing.Size(105,144);
            this.listViewJobs.TabIndex=2;
            this.listViewJobs.UseCompatibleStateImageBehavior=false;
            this.listViewJobs.View=System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text="Pending";
            this.columnHeader1.Width=100;
            // 
            // listViewJobsDone
            // 
            this.listViewJobsDone.Activation=System.Windows.Forms.ItemActivation.OneClick;
            this.listViewJobsDone.Anchor=((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top|System.Windows.Forms.AnchorStyles.Bottom)
                        |System.Windows.Forms.AnchorStyles.Left)));
            this.listViewJobsDone.AutoArrange=false;
            this.listViewJobsDone.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.listViewJobsDone.FullRowSelect=true;
            this.listViewJobsDone.GridLines=true;
            this.listViewJobsDone.HeaderStyle=System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewJobsDone.Location=new System.Drawing.Point(120,24);
            this.listViewJobsDone.MultiSelect=false;
            this.listViewJobsDone.Name="listViewJobsDone";
            this.listViewJobsDone.Size=new System.Drawing.Size(105,144);
            this.listViewJobsDone.TabIndex=3;
            this.listViewJobsDone.UseCompatibleStateImageBehavior=false;
            this.listViewJobsDone.View=System.Windows.Forms.View.Details;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text="Done";
            this.columnHeader2.Width=100;
            // 
            // listViewJobsError
            // 
            this.listViewJobsError.Activation=System.Windows.Forms.ItemActivation.OneClick;
            this.listViewJobsError.Anchor=((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top|System.Windows.Forms.AnchorStyles.Bottom)
                        |System.Windows.Forms.AnchorStyles.Left)));
            this.listViewJobsError.AutoArrange=false;
            this.listViewJobsError.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.listViewJobsError.ForeColor=System.Drawing.Color.Red;
            this.listViewJobsError.FullRowSelect=true;
            this.listViewJobsError.GridLines=true;
            this.listViewJobsError.HeaderStyle=System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewJobsError.Location=new System.Drawing.Point(232,24);
            this.listViewJobsError.MultiSelect=false;
            this.listViewJobsError.Name="listViewJobsError";
            this.listViewJobsError.Size=new System.Drawing.Size(105,144);
            this.listViewJobsError.TabIndex=4;
            this.listViewJobsError.UseCompatibleStateImageBehavior=false;
            this.listViewJobsError.View=System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text="Error";
            this.columnHeader3.Width=100;
            // 
            // groupBoxJobs
            // 
            this.groupBoxJobs.Anchor=((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top|System.Windows.Forms.AnchorStyles.Bottom)
                        |System.Windows.Forms.AnchorStyles.Left)
                        |System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxJobs.Controls.Add(this.nbJobslabel);
            this.groupBoxJobs.Controls.Add(this.label1);
            this.groupBoxJobs.Controls.Add(this.listViewJobs);
            this.groupBoxJobs.Controls.Add(this.listViewJobsDone);
            this.groupBoxJobs.Controls.Add(this.listViewJobsError);
            this.groupBoxJobs.Location=new System.Drawing.Point(288,16);
            this.groupBoxJobs.Name="groupBoxJobs";
            this.groupBoxJobs.Size=new System.Drawing.Size(352,224);
            this.groupBoxJobs.TabIndex=5;
            this.groupBoxJobs.TabStop=false;
            this.groupBoxJobs.Text="Jobs";
            // 
            // nbJobslabel
            // 
            this.nbJobslabel.Anchor=((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom|System.Windows.Forms.AnchorStyles.Left)));
            this.nbJobslabel.Location=new System.Drawing.Point(128,184);
            this.nbJobslabel.Name="nbJobslabel";
            this.nbJobslabel.Size=new System.Drawing.Size(100,23);
            this.nbJobslabel.TabIndex=6;
            this.nbJobslabel.Text="? / ?";
            // 
            // label1
            // 
            this.label1.Anchor=((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom|System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Location=new System.Drawing.Point(16,184);
            this.label1.Name="label1";
            this.label1.Size=new System.Drawing.Size(112,16);
            this.label1.TabIndex=5;
            this.label1.Text="Number of workers : ";
            // 
            // listViewMessage
            // 
            this.listViewMessage.Activation=System.Windows.Forms.ItemActivation.OneClick;
            this.listViewMessage.Anchor=((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom|System.Windows.Forms.AnchorStyles.Left)));
            this.listViewMessage.AutoArrange=false;
            this.listViewMessage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.iconeColumnHeader,
            this.messageColumnHeader});
            this.listViewMessage.FullRowSelect=true;
            this.listViewMessage.GridLines=true;
            this.listViewMessage.HeaderStyle=System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewMessage.LargeImageList=this.IconsList;
            this.listViewMessage.Location=new System.Drawing.Point(72,256);
            this.listViewMessage.MultiSelect=false;
            this.listViewMessage.Name="listViewMessage";
            this.listViewMessage.Size=new System.Drawing.Size(576,216);
            this.listViewMessage.SmallImageList=this.IconsList;
            this.listViewMessage.TabIndex=6;
            this.listViewMessage.UseCompatibleStateImageBehavior=false;
            this.listViewMessage.View=System.Windows.Forms.View.Details;
            // 
            // iconeColumnHeader
            // 
            this.iconeColumnHeader.Text="!";
            this.iconeColumnHeader.TextAlign=System.Windows.Forms.HorizontalAlignment.Center;
            this.iconeColumnHeader.Width=16;
            // 
            // messageColumnHeader
            // 
            this.messageColumnHeader.Text="Message";
            this.messageColumnHeader.Width=554;
            // 
            // IconsList
            // 
            this.IconsList.ImageStream=((System.Windows.Forms.ImageListStreamer)(resources.GetObject("IconsList.ImageStream")));
            this.IconsList.TransparentColor=System.Drawing.Color.Transparent;
            this.IconsList.Images.SetKeyName(0,"Gray.gif");
            this.IconsList.Images.SetKeyName(1,"Purple.gif");
            this.IconsList.Images.SetKeyName(2,"Red.gif");
            this.IconsList.Images.SetKeyName(3,"yellow.gif");
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
            // 
            // menuItem1
            // 
            this.menuItem1.Index=0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFileExit});
            this.menuItem1.Text="File";
            // 
            // menuItemFileExit
            // 
            this.menuItemFileExit.Index=0;
            this.menuItemFileExit.Text="Exit";
            this.menuItemFileExit.Click+=new System.EventHandler(this.menuItemFileExit_Click);
            // 
            // anubisPictureBox
            // 
            this.anubisPictureBox.Anchor=((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom|System.Windows.Forms.AnchorStyles.Left)));
            this.anubisPictureBox.Image=((System.Drawing.Image)(resources.GetObject("anubisPictureBox.Image")));
            this.anubisPictureBox.Location=new System.Drawing.Point(0,8);
            this.anubisPictureBox.Name="anubisPictureBox";
            this.anubisPictureBox.Size=new System.Drawing.Size(39,630);
            this.anubisPictureBox.TabIndex=7;
            this.anubisPictureBox.TabStop=false;
            // 
            // panel
            // 
            this.panel.Anchor=((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top|System.Windows.Forms.AnchorStyles.Bottom)
                        |System.Windows.Forms.AnchorStyles.Left)));
            this.panel.BackColor=System.Drawing.Color.FromArgb(((int)(((byte)(151)))),((int)(((byte)(115)))),((int)(((byte)(159)))));
            this.panel.Location=new System.Drawing.Point(0,0);
            this.panel.Name="panel";
            this.panel.Size=new System.Drawing.Size(39,32);
            this.panel.TabIndex=8;
            // 
            // Anubis
            // 
            this.AutoScaleDimensions=new System.Drawing.SizeF(6F,13F);
            this.AutoScaleMode=System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize=new System.Drawing.Size(664,633);
            this.ControlBox=false;
            this.Controls.Add(this.panel);
            this.Controls.Add(this.anubisPictureBox);
            this.Controls.Add(this.listViewMessage);
            this.Controls.Add(this.groupBoxJobs);
            this.Controls.Add(this.listViewPlugins);
            this.Menu=this.mainMenu;
            this.Name="Anubis";
            this.Text="Anubis";
            this.Closed+=new System.EventHandler(this.Anubis_Closed);
            this.groupBoxJobs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.anubisPictureBox)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

	}
}

