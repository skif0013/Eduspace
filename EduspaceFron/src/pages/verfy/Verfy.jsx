import React from 'react'
import HeaderRegister from '../../components/headerRegister/HeaderRegister'

const Verfy = () => {
	return (
		<div className='bg-gray-900 min-h-screen'>
			<div className='max-w-[1440px] m-auto flex items-center justify-center min-h-screen'>
				<div className='flex flex-col items-center'>
					<HeaderRegister register={"Password recovery"} />
				</div>
			</div>
		</div>
	)
}

export default Verfy
