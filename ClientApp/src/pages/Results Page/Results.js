import React, { Component } from "react"
import { Link } from "react-router-dom";
import Loader from "../../components/Loader/loader"
import './Results.css'
import SearchResult from "../../components/SearchResult/SearchResult";

export class ResultPage extends Component {
    displayName = ResultPage.name

    constructor(props) {
        super(props);
        this.state = { loadingState: false };
    }

    render() {
        return (
            < div className="results-page" >
                {
                    this.state.loadingState === true ? <Loader /> :
                        <main>
                            <div className="results-page-header">
                                <Link to="/" className="logo">
                                    <img src={require("../../ assets/logo.svg")} alt="Team logo" />
                                </Link>
                                <div className="result-info">
                                    <h2>Search results for ""</h2>
                                    <h5 className="matches-found">20 match(es) found</h5>
                                    <p className="response-time">Response time: </p>
                                </div>
                            </div>
                            <div className="results-body">
                                
                            </div>
                        </main>
                }
            </div >
        )
    }
}

export default ResultPage;