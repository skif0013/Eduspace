import { Routes, Route } from 'react-router-dom'

import AppContext from './Context/AppContext'

import Home from './pages/home/Home'

function App() {
	return (
		<AppContext.Provider>
			<Routes>
				<Route path='/' element={<Home />} />
			</Routes>
		</AppContext.Provider>
	)
}

export default App
