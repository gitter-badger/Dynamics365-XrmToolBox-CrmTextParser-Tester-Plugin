﻿using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yagasoft.CrmTextParserTesterPlugin.Control.Interfaces;
using Yagasoft.CrmTextParserTesterPlugin.Helpers;
using Yagasoft.CrmTextParserTesterPlugin.Model.Settings;
using Yagasoft.Libraries.Common;

namespace Yagasoft.CrmTextParserTesterPlugin.Control
{
	public partial class TemplateEditor : UserControl
	{
		private readonly WorkerHelper workerHelper;

		private bool isOutputShown;
		private UserControl currentControl;
		private UserControl editorControl;

		public TemplateEditor(WorkerHelper workerHelper)
		{
			this.workerHelper = workerHelper;
			InitializeComponent();
		}

		private void TemplateEditor_Load(object sender, EventArgs e)
		{
			ShowEditor();
		}

		public async void ShowEditor(bool isOutput = false)
		{
			isOutputShown = isOutput;

			var isHtml = isOutput ? checkBoxHtmlOutput.Checked : checkBoxHtmlEditor.Checked;

			var outputControl = editorControl == currentControl ? null : currentControl;
			currentControl = isHtml
				? (isOutput ? new BrowserOutputControl(this) : new BrowserEditorControl())
				: (isOutput ? new OutputControl(this) : new EditorControl());

			panelCodeEditor.Controls.Clear();
			panelCodeEditor.Controls.Add(currentControl);

			currentControl.Dock = DockStyle.Fill;

			SetEditorText(await (isOutput ? GetEditorText(outputControl) : GetEditorText()));

			if (!isOutput)
			{
				editorControl = currentControl;
			}
		}

		public async Task<string> GetEditorText(UserControl control = null)
		{
			return await (((control ?? editorControl) as IEditorAync)?.GetTextAsync()
				?? Task.FromResult(((control ?? editorControl) as IEditor)?.GetText()));
		}

		public void SetEditorText(string text)
		{
			(currentControl as IEditor)?.SetText(text);
		}

		private void checkBoxHtmlEditor_CheckedChanged(object sender, EventArgs e)
		{
			if (!isOutputShown)
			{
				ShowEditor();
			}
		}

		private void checkBoxHtmlOutput_CheckedChanged(object sender, EventArgs e)
		{
			if (isOutputShown)
			{
				ShowEditor(true);
			}
		}
	}
}
