import React from 'react'
import HeaderRegister from '../../components/headerRegister/HeaderRegister'
import AuthR from '../../components/authR/AuthR'

const Register = () => {
	return (
		<div className='bg-gray-900 min-h-screen'>
			<div className='max-w-[1440px] m-auto flex items-center justify-center min-h-screen'>
				<div className='flex flex-col items-center'>
					<HeaderRegister register={"Register"} />
					<AuthR />
				</div>
			</div>
		</div>
	)
}

export default Register
