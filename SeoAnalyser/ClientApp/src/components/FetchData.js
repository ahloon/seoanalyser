import React, { Component } from 'react';

export class FetchData extends Component {
	static renderForecastsTable(words) {
		return (
			<tbody>
				{words.map((word, index) =>
					<tr key={index}>
						<td>{word.word}</td>
						<td>{word.occurrences}</td>
					</tr>
				)}
			</tbody>
		);
	}

	static renderError() {
		return (
			<div><p>There was an error.</p></div>
		);
	}

	displayName = FetchData.name
	constructor(props) {
		super(props);
		this.state = { words: [], loading: true, text: "http://www.google.com", isUrl: false, inPage: false, inMeta: false, external: false, errorMsg: "" };
		this.sortBy.bind(this);
		this.handleTextChange = this.handleTextChange.bind(this);
		this.handleChange = this.handleChange.bind(this);
		this.fetchData();
	}

	compareBy(key) {
		return function (a, b) {
			if (a[key] < b[key]) return -1;
			if (a[key] > b[key]) return 1;
			return 0;
		};
	}

	sortBy(key) {
		let arrayCopy = [...this.state.words];
		arrayCopy.sort(this.compareBy(key));
		this.setState({ words: arrayCopy });
	}

	fetchData() {
		const encodedText = this.state.isUrl ? encodeURIComponent(this.state.text) : this.state.text;
		fetch(`api/SampleData/AnalyserResults/${encodedText}/${this.state.isUrl}/${this.state.inPage}/${this.state.inMeta}/${this.state.external}`)
			.then(this.handleErrors)
			.then(response => response.json())
			.then(data => {
				this.setState({ words: data, loading: false });
			})
			.catch(error => this.setState({ errorMsg: error.message }));
	}

	handleTextChange(event) {
		this.setState({ text: event.target.value }, () => { this.fetchData(); });
	}

	handleChange(event) {
		const target = event.target;
		const name = target.name;
		const value = target.type === 'checkbox' ? target.checked : target.value;
		this.setState({ [name]: value }, () => { this.fetchData(); });
		
	}

	handleErrors(response) {
		if (!response.ok) {
			throw Error(response.statusText);
		}
		return response;
	}

	render() {
		let contents = FetchData.renderForecastsTable(this.state.words);
		return (
			<div>
				<h1>SEO Analyser</h1>
				<p/>
				<textarea rows="4" cols="80" name="text" value={this.state.text} onChange={this.handleTextChange}/><br/>
				<input type="checkbox" name="isUrl" checked={this.state.isUrl} onChange={this.handleChange} /> This is a URL
				<input type="checkbox" name="inPage" checked={this.state.inPage} onChange={this.handleChange} /> Calculate in page 
				<input type="checkbox" name="inMeta" checked={this.state.inMeta} onChange={this.handleChange} /> Calculate in Meta tag 
				<input type="checkbox" name="external" checked={this.state.external} onChange={this.handleChange} /> Calculate external links<br />
				
				<table className='table'>
					<thead>
						<tr>
							<th onClick={() => this.sortBy('word')}>Word</th>
							<th onClick={() => this.sortBy('occurrences')}>Number of occurrences</th>
						</tr>
					</thead>
					{contents}
				</table>
			</div>
		);
	}
}
