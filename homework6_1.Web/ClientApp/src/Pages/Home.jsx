import { useEffect, useState, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { useUser } from '../UserContext';
import axios from 'axios';
import { HubConnectionBuilder } from "@microsoft/signalr";

const Home = () => {

    const navigate = useNavigate();
    const { user } = useUser();
    const [newTask, setNewTask] = useState('');
    const [tasks, setTasks] = useState([]);
    const connectionRef = useRef();

    useEffect(() => {
        { !user && navigate('/login') }
        {user && getTasks()}
    }, [])


    useEffect(() => {
        const connectToHub = async () => {
            const connection = new HubConnectionBuilder().withUrl("/api/task").build();
            await connection.start();
            connectionRef.current = connection;

            connection.on('newTask', value => {
                setTasks(tasks => [...tasks, value]);
            });

            connection.on('taskUpdate', value => {
                setTasks(value);
            });
        }
        connectToHub();
    }, []);

    const getTasks = async() => {
        const { data } = await axios.get('/api/task/getall')
        setTasks(data);
        console.log(data);
    }

    const onTextChange = (e) => {
        setNewTask(e.target.value);
    }

    const onAddClick = async() => {
        await axios.post('/api/task/addtask', { name: newTask });
        setNewTask('');
    }

    const imDoingClick = async (t) => {
        await axios.post('/api/task/updatetask', {id: t.id, name: t.name })
    }

    const imDoneClick = async (t) => {
        await axios.post('/api/task/deletetask', { id: t.id, name: t.name, userId: user.id})
    }

    return (
        <div>
            <div className="container">
                <div>
                    <div className="row">
                        <div className="col-md-10">
                            <input type="text" className="form-control" placeholder="Task Title" value={newTask} onChange={onTextChange}/>
                        </div>
                        <div className="col-md-2">
                            <button className="btn btn-primary w-100" onClick={onAddClick}>Add Task</button>
                        </div>
                    </div>
                    <table className="table table-hover table-striped table-bordered mt-3">
                        <thead>
                            <tr>
                                <th>Title</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            {tasks.map(t => (
                                <tr key={t.id}>
                                    <td>{t.name}</td>
                                    <td>
                                        {!(t.userId === user.id) && t.userId  && <button className="btn btn-warning" disabled>{t.user.firstName} {t.user.lastName} is doing this</button>}
                                        {!t.userId && <button className="btn btn-dark" onClick={()=>imDoingClick(t)}>I'm doing this one!</button>}
                                        {t.userId === user.id && <button className="btn btn-success" onClick={()=>imDoneClick(t)}>I'm done!</button>}
                                </td>
                            </tr>
                            )) }
                           
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};

export default Home;