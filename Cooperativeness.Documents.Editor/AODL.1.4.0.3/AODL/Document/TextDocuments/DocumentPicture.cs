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
using AODL.IO;

namespace AODL.Document.TextDocuments
{
	/// <summary>
	/// DocumentPicture represent a picture resp. graphic which used within
	/// a file in the OpenDocument format.
	/// </summary>
	public class DocumentPicture 
	{
	    private readonly IFile _file;

	    /// <summary>
	    /// Gets or sets the name of the image.
	    /// </summary>
	    /// <value>The name of the image.</value>
	    public string ImageName
	    {
	        get
	        {
	            return _file.Name;
	        }
	    }

	    public Stream OpenRead()
	    {
	        return _file.OpenRead();
	    }

		/// <summary>
		/// Initializes a new instance of the <see cref="DocumentPicture"/> class.
		/// </summary>
		/// <param name="file">The file.</param>
		public DocumentPicture(IFile file)
		{
		    _file = file;
		}
	}
}
