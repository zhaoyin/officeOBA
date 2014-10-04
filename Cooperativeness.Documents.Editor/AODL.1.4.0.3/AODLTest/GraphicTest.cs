/*************************************************************************
 *
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER
 * 
 * Copyright 2008 Sun Microsystems, Inc. All rights reserved.
 * 
 * Use is subject to license terms.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at http://www.apache.org/licenses/LICENSE-2.0. You can also
 * obtain a copy of the License at http://odftoolkit.org/docs/license.txt
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * 
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 ************************************************************************/

using System.IO;
using AODL.Document.Export.OpenDocument;
using AODL.Document.Import.OpenDocument;
using AODL.IO;
using NUnit.Framework;
using AODL.Document.TextDocuments;
using AODL.Document.Content.Text;
using AODL.Document.Content.Text.Indexes;
using AODL.Document.Content.Draw;
using AODL.Document.Content;
using AODL.Document.Styles;

namespace AODLTest
{
	[TestFixture]
	public class GraphicTest
	{
		private string _imagefile		= Path.Combine(AARunMeFirstAndOnce.inPutFolder, "Eclipse_add_new_Class.jpg");

		
		[Test]
		public void LoadGraphichAndSearchForAlternateText()
		{
			TextDocument document = new TextDocument();
            using (IPackageReader reader = new OnDiskPackageReader())
            {
                document.Load(AARunMeFirstAndOnce.inPutFolder + "ImageDocument.odt", new OpenDocumentImporter(reader));
            }
            Assert.AreEqual(3, document.Content.Count);
			Assert.AreEqual(true, document.Content[2] is Paragraph);
			Paragraph par = document.Content[2] as Paragraph;
			Assert.AreEqual(true, par.Content[0] is Frame);
			Frame frame = par.Content[0] as Frame;
			
			Assert.AreEqual("<alternative AODL text>", frame.AlternateText);
		}
		
		[Test]
		public void GraphicsTest()
		{
			TextDocument textdocument		= new TextDocument();
			textdocument.New();
			Paragraph p						= ParagraphBuilder.CreateStandardTextParagraph(textdocument);
			Frame frame						= new Frame(textdocument, "frame1",
				"graphic1", new DiskFile(_imagefile));
			p.Content.Add(frame);
			textdocument.Content.Add(p);
            using (IPackageWriter writer = new OnDiskPackageWriter())
            {
                textdocument.Save(AARunMeFirstAndOnce.outPutFolder + "grapic.odt", new OpenDocumentTextExporter(writer));
            }
		}

		[Test]
		public void Add2GraphicsWithSameNameFromDifferentLocations()
		{
			string file1 = _imagefile; //@"E:\fotos\schnee.jpg";
			string file2 = _imagefile; //@"E:\fotos\resize\schnee.jpg";
			TextDocument textdocument		= new TextDocument();
			textdocument.New();
			Paragraph p						= ParagraphBuilder.CreateStandardTextParagraph(textdocument);
			Frame frame						= new Frame(textdocument, "frame1",
                "graphic1", new DiskFile(file1));
			p.Content.Add(frame);
			Paragraph p1					= ParagraphBuilder.CreateStandardTextParagraph(textdocument);
			Frame frame1					= new Frame(textdocument, "frame2",
                "graphic2", new DiskFile(file2));
			p1.Content.Add(frame1);
			textdocument.Content.Add(p);
			textdocument.Content.Add(p1);
            using (IPackageWriter writer = new OnDiskPackageWriter())
            {
                textdocument.Save(AARunMeFirstAndOnce.outPutFolder + "graphic.odt", new OpenDocumentTextExporter(writer));
            }
		}

		/// <summary>
		/// Create a Illustration manuel.
		/// </summary>
		[Test]
		public void DrawTextBoxTest()
		{
			TextDocument textdocument		= new TextDocument();
			textdocument.New();
			Paragraph pOuter				= ParagraphBuilder.CreateStandardTextParagraph(textdocument);
			DrawTextBox drawTextBox			= new DrawTextBox(textdocument);
			Frame frameTextBox				= new Frame(textdocument, "fr_txt_box");
			frameTextBox.DrawName			= "fr_txt_box";
			frameTextBox.ZIndex				= "0";
			//			Paragraph pTextBox				= ParagraphBuilder.CreateStandardTextParagraph(textdocument);
			//			pTextBox.StyleName				= "Illustration";
			Paragraph p						= ParagraphBuilder.CreateStandardTextParagraph(textdocument);
			p.StyleName						= "Illustration";
			Frame frame						= new Frame(textdocument, "frame1",
                "graphic1", new DiskFile(_imagefile));
			frame.ZIndex					= "1";
			p.Content.Add(frame);
			p.TextContent.Add(new SimpleText(textdocument, "Illustration"));
			drawTextBox.Content.Add(p);
			
			frameTextBox.SvgWidth			= frame.SvgWidth;
			drawTextBox.MinWidth			= frame.SvgWidth;
			drawTextBox.MinHeight			= frame.SvgHeight;
			frameTextBox.Content.Add(drawTextBox);
			pOuter.Content.Add(frameTextBox);
			textdocument.Content.Add(pOuter);
            using (IPackageWriter writer = new OnDiskPackageWriter())
            {
                textdocument.Save(AARunMeFirstAndOnce.outPutFolder + "drawTextbox.odt", new OpenDocumentTextExporter(writer));
            }
		}

		[Test]
		public void CreateIllustrationUsingTheFrameBuilder()
		{
			TextDocument document			= new TextDocument();
			document.New();
			//Create a standard pargraph for the illustration
			Paragraph paragraphStandard		= ParagraphBuilder.CreateStandardTextParagraph(document);
			//Create Illustration Frame using the FrameBuilder
			Frame frameIllustration			= FrameBuilder.BuildIllustrationFrame(
				document,
				"illustration_frame_1",
				"graphic1",
                new DiskFile(_imagefile),
				"This is a Illustration",
				1);
			//Add the Illustration Frame to the Paragraph
			paragraphStandard.Content.Add(frameIllustration);
			Assert.IsTrue(frameIllustration.Content[0] is DrawTextBox, "Must be a DrawTextBox!");
			Assert.IsTrue(((DrawTextBox)frameIllustration.Content[0]).Content[0] is Paragraph, "Must be a Paragraph!");
			Paragraph paragraph				= ((DrawTextBox)frameIllustration.Content[0]).Content[0] as Paragraph;
			Assert.IsTrue(paragraph.TextContent[1] is TextSequence, "Must be a TextSequence!");
			//Add Paragraph to the document
			document.Content.Add(paragraphStandard);
			//Save the document
            using (IPackageWriter writer = new OnDiskPackageWriter())
            {
                document.Save(AARunMeFirstAndOnce.outPutFolder + "illustration.odt", new OpenDocumentTextExporter(writer));
            }
		}

		/// <summary>
		/// Image map test. Desc: Create new Text document, create a new frame with
		/// graphic. Create a DrawAreaRectangle and a DrawAreaCircle and them
		/// as Image Map to the graphic.
		/// TODO: change imagePath!
		/// </summary>
		[Test]
		public void ImageMapTest()
		{
			TextDocument document			= new TextDocument();
			document.New();
			//Create standard paragraph
			Paragraph paragraphOuter		= ParagraphBuilder.CreateStandardTextParagraph(document);
			//Create the frame with graphic
            Frame frame = new Frame(document, "frame1", "graphic1", new DiskFile(_imagefile));
			//Create a Draw Area Rectangle
			DrawAreaRectangle drawAreaRec	= new DrawAreaRectangle(
				document, "0cm", "0cm", "1.5cm", "2.5cm", null);
			drawAreaRec.Href				= "http://OpenDocument4all.com";
			//Create a Draw Area Circle
			DrawAreaCircle drawAreaCircle	= new DrawAreaCircle(
				document, "4cm", "4cm", "1.5cm", null);
			drawAreaCircle.Href				= "http://AODL.OpenDocument4all.com";
			DrawArea[] drawArea				= new DrawArea[2] { drawAreaRec, drawAreaCircle };
			//Create a Image Map
			ImageMap imageMap				= new ImageMap(document, drawArea);
			//Add Image Map to the frame
			frame.Content.Add(imageMap);
			//Add frame to paragraph
			paragraphOuter.Content.Add(frame);
			//Add paragraph to document
			document.Content.Add(paragraphOuter);
			//Save the document
            using (IPackageWriter writer = new OnDiskPackageWriter())
            {
                document.Save(AARunMeFirstAndOnce.outPutFolder + "simpleImageMap.odt", new OpenDocumentTextExporter(writer));
            }
		}

		[Test]
		public void LoadGraphicAccessGraphic()
		{
			TextDocument document			= new TextDocument();
            using (IPackageReader reader = new OnDiskPackageReader())
            {
                document.Load(AARunMeFirstAndOnce.inPutFolder + "hallo.odt", new OpenDocumentImporter(reader));

                foreach (IContent iContent in document.Content)
                {
                    if (iContent is Paragraph && ((Paragraph) iContent).Content.Count > 0
                        && ((Paragraph) iContent).Content[0] is Frame)
                    {
                        Frame frame = ((Paragraph) iContent).Content[0] as Frame;
                        Assert.IsTrue(frame.Content[0] is Graphic, "Must be a graphic!");
                        Graphic graphic = frame.Content[0] as Graphic;
                        //now access the full qualified graphic path
                        Assert.IsNotNull(graphic.GraphicFile, "The graphic real path must exist!");
                        //Assert.IsTrue(File.Exists(graphic.GraphicRealPath));
                    }
                }
                using (IPackageWriter writer = new OnDiskPackageWriter())
                {
                    document.Save(AARunMeFirstAndOnce.outPutFolder + "hallo_rel_graphic_touch.odt", new OpenDocumentTextExporter(writer));
                }
            }
		}

		[Test]
        [Ignore("No reasonable way to test this")]
		public void LoadFileDeleteGraphic()
		{
			string fileName					= "hallo_rem_graphic.odt";
			string graphicFile				= "";
			TextDocument document			= new TextDocument();
            using (IPackageReader reader = new OnDiskPackageReader())
            {
                document.Load(AARunMeFirstAndOnce.inPutFolder + "hallo.odt", new OpenDocumentImporter(reader));

                foreach (IContent iContent in document.Content)
                {
                    if (iContent is Paragraph && ((Paragraph) iContent).Content.Count > 0
                        && ((Paragraph) iContent).Content[0] is Frame)
                    {
                        Frame frame = ((Paragraph) iContent).Content[0] as Frame;
                        Assert.IsTrue(frame.Content[0] is Graphic, "Must be a graphic!");
                        Graphic graphic = frame.Content[0] as Graphic;
                        //now access the full qualified graphic path
                        Assert.IsNotNull(graphic.GraphicFile, "The graphic real path must exist!");
                        //Assert.IsTrue(File.Exists(graphic.GraphicRealPath));
                        //graphicFile = graphic.GraphicRealPath;
                        //Delete the graphic
                        //frame.Content.Remove(graphic);
                    }
                }
                using (IPackageWriter writer = new OnDiskPackageWriter())
                {
                    document.Save(AARunMeFirstAndOnce.outPutFolder + fileName, new OpenDocumentTextExporter(writer));
                }
            }
		    //Special case, only for this test neccessary
			document.Dispose();
			//Load file again
			TextDocument documentRel		= new TextDocument();
            using (IPackageReader reader = new OnDiskPackageReader())
            {
                documentRel.Load(AARunMeFirstAndOnce.outPutFolder + fileName, new OpenDocumentImporter(reader));
            }
			Assert.IsFalse(File.Exists(graphicFile), "This file must be removed from this file!");
		}

		[Test]
		public void CreateFreePositionGraphic()
		{
			TextDocument textdocument		= new TextDocument();
			textdocument.New();
			Paragraph p						= ParagraphBuilder.CreateStandardTextParagraph(textdocument);
			Frame frame						= FrameBuilder.BuildStandardGraphicFrame(textdocument, "frame1",
                "graphic1", new DiskFile(_imagefile));
			//Setps to set a graphic free with x and y
			frame.SvgX						= "1.75cm";
			frame.SvgY						= "1.75cm";
			((FrameStyle)frame.Style).GraphicProperties.HorizontalPosition		= "from-left";
			((FrameStyle)frame.Style).GraphicProperties.VerticalPosition		= "from-top";
			((FrameStyle)frame.Style).GraphicProperties.HorizontalRelative		= "paragraph";
			((FrameStyle)frame.Style).GraphicProperties.VerticalRelative		= "paragraph";
			p.Content.Add(frame);
			textdocument.Content.Add(p);
            using (IPackageWriter writer = new OnDiskPackageWriter())
            {
                textdocument.Save(AARunMeFirstAndOnce.outPutFolder + "grapic_free_xy.odt", new OpenDocumentTextExporter(writer));
            }
		}
	}
}
