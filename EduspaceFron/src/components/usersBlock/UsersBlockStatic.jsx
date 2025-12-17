import React from 'react'

function UsersBlockStatic() {
	return (
		<div className='m-auto mt-20'>
			<div className='flex gap-25 items-center justify-center'>
				<div className='flex items-center justify-center flex-col'>
					<h2 className='text-white text-3xl font-bold'>3,800+</h2>
					<p className='text-blue-950 text-base font-medium'>Users Active</p>
				</div>
				<div className='h-20 bg-gray-600 w-0.5'></div>
				<div className='flex items-center justify-center flex-col'>
					<h2 className='text-white text-3xl font-bold'>230+</h2>
					<p className='text-blue-950 text-base font-medium'>Trusted Componies</p>
				</div>
				<div className='h-20 bg-gray-600 w-0.5'></div>
				<div className='flex items-center justify-center flex-col'>
					<h2 className='text-white text-3xl font-bold'>$230M+</h2>
					<p className='text-blue-950 text-base font-medium'>Total Transactions</p>
				</div>
			</div>
		</div>
	)
}

export default UsersBlockStatic