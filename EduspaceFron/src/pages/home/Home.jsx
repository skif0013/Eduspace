import React from 'react'
import Header from '../../components/header/Header'
import Slider from '../../components/slider/Slider'

const Home = () => {
	return (
		<div className='bg-gray-900 min-h-screen'>
			<div className='max-w-[1440px] m-auto'>
				<Header />
				<Slider />
			</div>
		</div>
	)
}

export default Home
