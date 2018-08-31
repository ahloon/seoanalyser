import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { FetchData } from './components/FetchData';

export default class App extends Component {
	displayName = App.name

	render() {
		return (
			<Layout>

				<Route path='/fetchdata' component={FetchData} />
			</Layout>
		);
	}
}
