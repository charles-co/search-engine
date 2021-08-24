import React, { Component } from "react"
import { Link } from "react-router-dom"
import "./Upload.css"

export class UploadPage extends Component {
    displayName = UploadPage.name

    constructor(props) {
        super(props);
        this.state = { loadingState: false };
    }

    render() {
        return (
            <div className="upload-page">
                <div className="upload-page-header">
                    <Link to="/" className="logo">
                        <img src={require("../../ assets/logo.svg")} alt="team logo" />
                    </Link>
                    <div className="upload-page-info">
                        <h3 className="title">Upload content</h3>
                        <p className="subtitle">Fill the form with the appropriate information required</p>
                    </div>
                </div>
                <div className="upload-page-main">
                    <form>
                        <div className="upload-form-input">
                            <label htmlFor="">Title:</label>
                            <input type="text" name="title" id="document-title" required />
                        </div>
                        <div className="upload-form-input">
                            <label htmlFor="">Description:</label>
                            <textarea name="description" id="document-desc"></textarea>
                        </div>
                        <div className="upload-form-input">
                            <label htmlFor="document">Choose a file to upload</label>
                            <input type="file" name="document" id="document" required/>
                        </div>
                        <input type="submit" value="Upload" className="submit-btn"/>
                    </form>
                    <p className="notice">
                        N.B: This information will be available on the search engine about 2 hours after upload
                    </p>
                </div>
            </div>
            )
    }

}

