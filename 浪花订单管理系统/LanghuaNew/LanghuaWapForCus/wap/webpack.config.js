var  path = require('path');
var  webpack = require('webpack');
var  HtmlWebpackPlugin = require('html-webpack-plugin');
var  ExtractTextPlugin = require('extract-text-webpack-plugin');
var  CleanPlugin = require('clean-webpack-plugin');
var TransferWebpackPlugin = require('transfer-webpack-plugin');

config = {
	entry: {
		bundle: [
			//'webpack/hot/dev-server',
			//'webpack-dev-server/client?http://localhost:8080',
			path.resolve(__dirname, './app/main.js')
		],
		vendors: [
			'react',
			'react-dom',
			//'redux',
			//'element-dataset',
			path.resolve(__dirname, './lib/common.js')
		]
	},
	output: {
		path: path.resolve(__dirname,'dist'),
		filename: '[name].js'
	},
	module: {
		loaders: [
			{
                test: /\.jsx?$/,
                loader: 'babel',
                exclude: /node_modules/,
                query:{
                    presets: ['es2015','react','stage-0'],
                    plugins: ['transform-runtime']
                }
            }, {
				test: /\.scss$/,
				loader: 'style-loader!css-loader!sass-loader'
			}, {
				test: /\.css$/,
				loader: ExtractTextPlugin.extract('style-loader', 'css-loader', 'postcss-loader')
			}, {
				test: /\.json$/,
				loader: 'json-loader'
			}, {
				test: /\.(woff|woff2|eot|ttf|svg|otf)$/,
				loader: 'file?name=fonts/[name].[ext]'
			},{
				test: /\.(png|jpg|gif)$/,
				loader: 'url-loader?limit=8192&name=images/[name].[ext]'
			}
		]
	},

	resolve: {
		extensions: ['', '.js', '.jsx', 'css', 'scss'],
		alias: {
			'react': path.resolve(__dirname, './node_modules/react/dist/react.js'),
			'react-dom': path.resolve(__dirname, './node_modules/react-dom/dist/react-dom.js'),
			//'redux': path.resolve(__dirname, './node_modules/redux/dist/redux.js'),
		}
	},
	plugins: [
		//new webpack.HotModuleReplacementPlugin(),
		new HtmlWebpackPlugin({
			filename: 'index.html',
			template: path.resolve(__dirname, 'index.html'),
			inject: true
		}),
		new ExtractTextPlugin('style.css', {
			allChunks: true,
			disable: false
		}),
		//new CleanPlugin(['dist']),
		new TransferWebpackPlugin( [{from: 'images'} ], path.resolve(__dirname,"dist"))
	]
};

if( process.env.NODE_ENV === 'prod' ){

	config.entry.bundle = [
		path.resolve(__dirname, './app/main.js')
	];

	config.output.filename = '[name].[hash].js';

	config.plugins = [
		new HtmlWebpackPlugin({
			filename: 'index.html',
			template: path.resolve(__dirname, 'index.html'),
			inject: true
		}),
		new ExtractTextPlugin('style.[chunkhash].css', {
			allChunks: true,
			disable: false
		}),
		new CleanPlugin(['dist']),
		new webpack.optimize.UglifyJsPlugin({
		  compress: {
		    warnings: false
		  }
		})
	]
}


module.exports = config;
