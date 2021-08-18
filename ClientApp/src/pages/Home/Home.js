import React, { Component } from 'react';
import './Home.css';

export class Home extends Component {
    displayName = Home.name
    
   render() {
       return (
           <div className="landing-page">
               <div className="page-header">
                   <div className="logo-homepage">
                       <img src={require("../../ assets/logo.svg")} className="logo-img" alt="Team logo" />
                   </div>
                   <p className="page-subtitle"> A custom search engine built with React & C# </p>
                   <form className="search-form">
                       <input type="text" className="search-input" placeholder="Search for anything..." />
                       <input type="image" name="submit" className="search-submit-btn" border="0" alt="Submit" src={require("../../ assets/search.svg")} />
                   </form>
               </div>
               <div className="supported-content">
                   <h3>This search engine allows you search for </h3>
                   <div className="supported-items">
                       <div className="supported-item">
                           <img src={require("../../ assets/txt.svg")} className="supported-item-img" alt="txt" />
                       </div>
                       <div className="supported-item">
                           <img src={require("../../ assets/excel.svg")} className="supported-item-img" alt="excel" />
                       </div>
                       <div className="supported-item">
                           <img src={require("../../ assets/powerpoint.svg")} className="supported-item-img" alt="powerpoint" />
                       </div>
                       <div className="supported-item">
                           <img src={require("../../ assets/word.svg")} className="supported-item-img" alt="word" />
                       </div>
                       <div className="supported-item">
                           <img src={require("../../ assets/json.svg")} className="supported-item-img" alt="json" />
                       </div>
                   </div>
                   <p>and more</p>
               </div>
           </div>
    );
  }
}
