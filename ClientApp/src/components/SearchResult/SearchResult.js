import React from "react"
import "./SearchResult.css"

const SearchResult = () => {
    return (
        <div className="search-result">
            <div className="search-result-img">
                <img src={require("../../ assets/txt.svg")} alt="doc type" />
            </div>
            <div className="search-result-info">
                <a className="title">Title</a>
                <p className="description">Desc</p>
            </div>
        </div>
     )
}

export default SearchResult