import Vue from 'vue'
import editor from 'vue2-editor'

Vue.use(editor);

Vue.config.productionTip = false;
Vue.config.devtools = true;

export const extend = function () {

	// Variables
	var extended = {};
	var deep = false;
	var i = 0;

	// Check if a deep merge
	if (typeof (arguments[0]) === 'boolean') {
		deep = arguments[0];
		i++;
	}

	// Merge the object into the extended object
	var merge = function (obj) {
		for (var prop in obj) {
			if (obj.hasOwnProperty(prop)) {
				if (deep && Object.prototype.toString.call(obj[prop]) === '[object Object]') {
					// If we're doing a deep merge and the property is an object
					extended[prop] = extend(true, extended[prop], obj[prop]);
				} else if (deep &&  Object.prototype.toString.call(obj[prop]) === '[object Array]') {
					// If we're doing a deep merge and the property is an array
					if (extended[prop] === undefined) { 
						// first argument needs to be initialized.
						extended[prop] = []; 
					}
					// loop through array and merge the elements
					for (var element in obj[prop]) {
						extended[prop][element] = extend(true, extended[prop][element], obj[prop][element]);
					}
				}else {
					// Otherwise, do a regular merge
					extended[prop] = obj[prop];
				}
			}
		}
	};

	// Loop through each object and conduct a merge
	for (; i < arguments.length; i++) {
		merge(arguments[i]);
	}

	return extended;
};

export const GenerateUniqueID = function () {
	let length = 8;
	let timestamp = new Date().valueOf();

	let ts = timestamp.toString();
	let parts = ts.split( "" ).reverse();
	let id = "";

	for ( var i = 0; i < length; ++i) {
		let index = Math.floor( Math.random() * ( (parts.length - 1) + 1) );
		id += parts[index];
	}

	return id;
}