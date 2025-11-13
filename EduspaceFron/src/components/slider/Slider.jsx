import React from 'react'
import './Slider.scss'

const Slider = () => {
	return (
		<div className='flex gap-6 py-20 items-center justify-between text-center lg:flex-row flex-col'>
			<div className='flex flex-col items-center'>
				<h1 className='text-white text-2xl font-bold sm:text-4xl'>
					About Our Educational Platform
				</h1>
				<div className='max-w-[720px] flex flex-col gap-8 mt-17'>
					<p className='wapErP'>
						Our platform provides modern and accessible educational resources
						for students, beginners, and professionals who want to improve their
						skills. We focus on delivering clear explanations, structured
						materials, and practical tasks that help users learn effectively.
					</p>
					<p className='wapErP'>
						We offer courses, guides, and interactive lessons on various topics,
						including programming, design, technology, and personal development.
						Each section is created to support learners at different levels,
						from beginners to advanced users.
					</p>
					<p className='wapErP'>
						Our goal is to make education simple, understandable, and available
						to everyone. We continue to update our materials and develop new
						features to ensure a high-quality learning experience.
					</p>
					<p className='wapErP'>
						If you have questions or suggestions, you can reach us through the
						contact page.
					</p>
				</div>
				<button
					className=' mt-17 py-3 px-12 rounded-2xl text-white border border-blue-900
						transition-all duration-300 ease-in-out
						hover:bg-blue-900 hover:border-blue-950 hover:shadow-lg hover:shadow-blue-500/30'
				>
					Learn More
				</button>
			</div>
			<div className='overflow-hidden rounded-4xl'>
				<img
					className='w-[520px]  rounded-4xl 
			transition-all duration-700 ease-out
			hover:scale-110 hover:translate-y-1'
					src='/learn.jpeg'
					alt=''
				/>
			</div>
		</div>
	)
}

export default Slider
