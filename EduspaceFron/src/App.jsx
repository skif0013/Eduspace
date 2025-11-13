import { Routes, Route } from 'react-router-dom'

import AppContext from './Context/AppContext'

import Home from './pages/home/Home'
import NotFound from './pages/notFound/NotFound'

function App() {
	return (
		<AppContext.Provider value={AppContext}>
			<Routes>
				<Route path='/' element={<Home />} />
				<Route path='*' element={<NotFound/>} />
			</Routes>
		</AppContext.Provider>
	)
}

export default App
