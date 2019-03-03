import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Sentiment } from './components/Sentiment/Sentiment';
import { TriangleArbitrage } from './components/Arbitrage/TriangleArbitrage';
import { NormalArbitrage } from './components/Arbitrage/NormalArbitrage';
import { Settings } from './components/Settings/Settings';
import { Portfolio } from './components/Portfolio/Portfolio';
import { BotDashboard } from './components/Bots/BotDashboard/BotDashboard';
import { TemplateChooser } from './components/Bots/TemplateChooser/TemplateChooser';

export default class App extends Component {
  displayName = App.name

  render() {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route exact path='/bots' component={BotDashboard} />
        <Route exact path='/bots/new' component={TemplateChooser} />
        <Route exact path='/sentiment' component={Sentiment} />
        <Route exact path='/arbitrage/normal' component={NormalArbitrage} />
        <Route exact path='/arbitrage/triangle' component={TriangleArbitrage} />
        <Route exact path='/portfolio' component={Portfolio} />
        <Route exact path='/settings' component={Settings} />
      </Layout>
    );
  }
}
