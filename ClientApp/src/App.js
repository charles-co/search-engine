import React, { Component } from 'react';
import { Route, Router, Switch } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './pages/Home/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';
import Homepage from './pages/Homepage';

export default class App extends Component {
  displayName = App.name

  render() {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/counter' component={Counter} />
        <Route path='/fetchdata' component={FetchData} />
        <Route path="/home" component={Homepage}/>
      </Layout>
    );
  }
}
